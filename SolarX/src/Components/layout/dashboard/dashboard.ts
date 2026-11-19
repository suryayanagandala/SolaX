import { Component, OnDestroy, OnInit,AfterViewInit,ViewChild } from '@angular/core';
import { SolarPerformanceService } from '../../../Shared/solar-performance';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { TokenService } from '../../../Shared/token';
import { interval,max,Subscription } from 'rxjs';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { ChartComponent, NgApexchartsModule } from "ng-apexcharts";
import { PowerFormatPipe } from '../../../CustomePipes/PowerPipe';
import { CurrencyPipe } from '../../../CustomePipes/CurrencyPipe';
import {ApexAxisChartSeries,ApexChart,ApexXAxis,ApexTitleSubtitle,ApexStroke,ApexNonAxisChartSeries
  ,ApexPlotOptions,ApexMarkers,ApexAnnotations
} from "ng-apexcharts";
import { SharedThemeService } from '../../../Shared/theme-service';
import { marker } from 'leaflet';
export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
  markers: ApexMarkers;
};
export type MonthlyChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  title: ApexTitleSubtitle;
  tooltip: ApexTooltip;
  dataLabels: ApexDataLabels;
  plotOptions:ApexPlotOptions
};

export type RadialChartOptions={
  series:ApexNonAxisChartSeries,
  chart:ApexChart,
  plotOptions:ApexPlotOptions,
  theme: ApexTheme; 
  labels:string[]
};

export type ForecastChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
  markers: ApexMarkers;
  theme: ApexTheme;
  annotations: ApexAnnotations;
}
export type DailyChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  title: ApexTitleSubtitle;
  dataLabels: ApexDataLabels;
  theme: ApexTheme;
  annotations: ApexAnnotations; 
  // colors:ApexColorStop
};

@Component({
  selector: 'app-dashboard',
  standalone:true,
  imports: [MaterialModule,CommonModule,NgApexchartsModule,PowerFormatPipe,CurrencyPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
 
})
export class Dashboard implements OnInit,AfterViewInit,OnDestroy{
  
latestData!:any;
refreshData!:Subscription;
performanceList!:any[];
public chartOptions!:ChartOptions;
public monthlyChart!:MonthlyChartOptions;
public radialChart!:RadialChartOptions;
public dailyweatherChart!:DailyChartOptions;
summaryDashbaord!:any;
timeWindowMinutes = 30; 

forecastData: any[] = [];
labels: string[] = [];
values: number[] = [];
@ViewChild("chart") chart!: ChartComponent;
chartData: { x: number; y: number }[] = [];

powerData: number[] = [];
timeData: string[] = [];

monthlyPower:number[]=[];
monthlyDates:string[]=[];

activatedPanles!:any[];
selectedPanelId!:any;
panelSeriesMap: Map<number, any[]> = new Map();
panelDetails!:any;
isChartLoading = false;
isDarkMode: boolean = false;

dailyWeatherLables!:any[];
dailyWeatherValues!:any[];
dailyWeatherData!:any[];
public forecastChartOptions!: Partial<ForecastChartOptions>;
constructor(private service:SolarPerformanceService,private toastr:ToastrService,
  private router:Router,private token:TokenService,private signalService:NotificationSignalrService,
  private themeService:SharedThemeService
){}
ngOnInit(): void {
  const userId=this.token.GetUserId();
  
  if(!userId){
    this.router.navigate(['/login'],{
      queryParams:{returnUrl:this.router.url}
    })
  }

   
   this.initializeEmptyChart();
   this.summaryDashboardForCusomer();
   this.getActivatedSolarPanles();
   this.getWeatherForecast();
   this.dailyWeatherChart();
    this.themeService.isDarkMode$.subscribe(mode => {
    this.isDarkMode = mode;
    this.buildChart(); 
    this.buildDailyWeatherInputChart();
  });
}
 

 ngAfterViewInit(): void {
    this.loadInitialData();
    this.listenToRealUpdates();
  
 }

 ngOnDestroy(): void {
  this.signalService.stopConnection(); 
 }

 
 initializeEmptyChart(){
  this.chartOptions={
    series:[
      {
        name:"Power (W)",
        data:[],
        
      }
    ],
    chart:{
      type:'line',
      height:350,
      toolbar: { show: false },
      zoom: { enabled: false },
      foreColor: "#ffffff",
      animations:{
        enabled:true,
        speed:800,
        
        
      }
    },
    stroke:{
      curve:"smooth",
      width:3,
      
    },
    markers:{
      size: 4
    },
    xaxis:{
      type:"datetime",
       labels: {
    datetimeUTC: false,
    
  }
    },
    title:{
      text:"Live Solar Power Output"
    },
    
  }
 }

 loadInitialData(panelId?: number) {

  this.isChartLoading = true;
  this.panelSeriesMap.clear();

  this.chartOptions = {
    ...this.chartOptions,
    series: []
  };

  this.service.getPerformance(panelId).subscribe((res: any) => {

    if (res.status && Array.isArray(res.data)) {

      res.data.forEach((item: any) => {

        const pId = item.panelId;

        if (!this.panelSeriesMap.has(pId)) {
          this.panelSeriesMap.set(pId, []);
        }

        const point = {
          x: new Date(item.generated_at).getTime(),
          y: item.power
        };

        this.panelSeriesMap.get(pId)!.push(point);
      });

      this.panelSeriesMap.forEach((data, key) => {
        this.panelSeriesMap.set(key, this.filterLast30Minutes(data));
      });

      const colors = ["#00E396", "#FEB019", "#FF4560", "#775DD0"];

      const series = Array.from(this.panelSeriesMap.entries()).map(
        ([id, data], index) => ({
          name: `Panel ${id}`,
          data: data,
          color: colors[index % colors.length]
        })
      );

      this.chartOptions = {
        ...this.chartOptions,
        series
      };
    }

    this.isChartLoading = false;

  }, () => {
    this.isChartLoading = false;
  });
}


 private filterLast30Minutes(data: { x: number; y: number }[]) {

  const now = new Date().getTime();

  const windowMs = this.timeWindowMinutes * 60 * 1000;

  return data.filter(point => now - point.x <= windowMs);
}



listenToRealUpdates() {
  this.signalService.onReceivePerformance((data) => {
    console.log('solar performance dto',data);
    
    const panelId = data.panelId;

    console.log(panelId);
    
    
    if (this.selectedPanelId !== null && panelId !== this.selectedPanelId) {
      return;
    }

    if (!this.panelSeriesMap.has(panelId)) {
      this.panelSeriesMap.set(panelId, []);
    }

    let panelData = this.panelSeriesMap.get(panelId)!;

    const point = {
      x: new Date(data.generated_at).getTime(),
      y: data.power
    };

    panelData.push(point);

    panelData = this.filterLast30Minutes(panelData);

    this.panelSeriesMap.set(panelId, panelData);

    
    const colors = ["#00E396", "#FEB019", "#FF4560", "#775DD0"];

    const series = Array.from(this.panelSeriesMap.entries()).map(
      ([id, data], index) => ({
        name: `Panel ${id}`,
        data: data,
        color: colors[index % colors.length]
      })
    );

    if (this.chart) {
      this.chart.updateSeries(series);
    }
  });
}
  loadMonthlyUpdates(panelId?:number){
    this.service.getMonthlyUpdates(panelId).subscribe((res)=>{
      if(res.status){
        this.monthlyPower=Array.isArray(res.data)?res.data.map((item:any)=>item.totalPower):[],
        this.monthlyDates=Array.isArray(res.data)?res.data.map((item:any)=>
        new Date(item.date).toDateString()):[]
         console.log('monthly power',this.monthlyPower);
         console.log('monthlyDate',this.monthlyDates);
         
         
        this.initializeBarChart();
      }
    })
  }
  private formatPower(value: number): string {
  if (value < 1) return "0 W";
  if (value >= 1000000) {
    return (value / 1000000).toFixed(2) + " MW";
  }
  if (value >= 1000) {
    return (value / 1000).toFixed(2) + " kW";
  }
  return value.toFixed(0) + " W";
}
  
 initializeBarChart() {
  this.monthlyChart = {
    series: [
      {
        name: "Monthly Production",
        data: this.monthlyPower
      }
    ],
    chart: {
      type: "bar",
      height: 350,
      foreColor: "#ffffff"
    },
    plotOptions: {
      bar: {
        dataLabels: {
          position: "top" 
        }
      }
    },
    xaxis: {
      categories: this.monthlyDates,
      title: {
        text: "Month"
      }
    },
    yaxis: {
      title: {
        text: "Power (W)"
      },
      labels: {
        formatter: (value: number) => {
          return this.formatPower(value);
        }
      }
    },
    tooltip: {
      y: {
         formatter: (value: number) => {
          return this.formatPower(value);
        }
      }
    },
    dataLabels: {
      enabled: true,
      offsetY: -30, 
  style: {
    fontSize: "12px"
  },
       formatter: (value: number) => {
          return this.formatPower(value);
      }
    },
    title: {
      text: "Monthly Solar Production"
    }
  };
}

summaryDashboardForCusomer(panelId?:number) {
  this.service.solarSummaryDashboard(panelId).subscribe((res) => {
    if (res.status) {
      this.summaryDashbaord = res.data;

      console.log(" Summary Loaded:", this.summaryDashbaord);

      
      if (this.selectedPanelId === null) {
        this.updateOverallEfficiency();
      }
    }
  });
}

getActivatedSolarPanles() {
  this.service.getActivatedSolarPanles().subscribe((res) => {
    if (res.status) {
      this.activatedPanles = res.data ?? [];
      this.selectedPanelId = null;

      
      this.loadMonthlyUpdates();
    }
  });
}

initialRadialChart(value:number){
  this.radialChart={
    series:[value],
    chart:{
      type:"radialBar",
      height:250,
      background: "transparent",
      animations: {
    enabled: true,
    
    speed: 800
  }
    },
     theme: {
      mode: "dark"  
    },
    plotOptions:{
      radialBar:{
        hollow:{
          size:"60"
        },
        dataLabels:{
          name:{
            show:true,
            color:"#fff"
          },
          value:{
            show: true,
            color: "#4caf50",
            fontSize: "22px"
          }
        }
      }
    },
    labels:["Efficiency"]
  }
}

onPanelChange(panelId: number | null) {

  this.selectedPanelId = panelId;

  console.log("Selected Panel:", panelId);

  
  this.panelSeriesMap.clear();

  if (this.chart) {
    this.chart.updateSeries([]); 
  }

  if (panelId !== null) {

    this.service.getPanelDetails(panelId).subscribe((res) => {
      if (res.status) {
        this.panelDetails = res.data;

        
        this.initialRadialChart(this.panelDetails.efficiency);

        this.loadMonthlyUpdates(panelId);
        this.loadInitialData(panelId);
        this.summaryDashboardForCusomer(panelId);
      }
    });

  } else {

    this.panelDetails = null;

    console.log("ALL PANELS selected");

    this.updateOverallEfficiency();

    this.loadMonthlyUpdates();
    this.loadInitialData(); 
  }
}

  updateOverallEfficiency() {
  if (!this.summaryDashbaord) {
    console.log(" Summary not ready");
    return;
  }

  const efficiency = Number(
    this.summaryDashbaord.averageEfficiency?.toFixed(2)
  );

  console.log(" Setting Overall Efficiency:", efficiency);

  this.initialRadialChart(efficiency);
}
   getWeatherForecast(){
    this.service.getWeatherForecast().subscribe((res)=>{
      if(res.status){
         const data = res.data.hourlyPredictions;
         console.log('Weather -reports',res.data);
         const sorted = data.sort((a:any, b:any) => {
  return this.convertTo24Hour(a.time) - this.convertTo24Hour(b.time);
});
         
         this.labels = sorted.map((x: { time: string; power: number }) => x.time);
         this.values = sorted.map((x: { time: string; power: number }) => x.power);
         this.buildChart();
      }
      else{

      }
    })
   }
   
   convertTo24Hour(time: string): number {
  const [hour, modifier] = time.split(' ');

  let h = parseInt(hour);

  if (modifier === 'PM' && h !== 12) {
    h += 12;
  }

  if (modifier === 'AM' && h === 12) {
    h = 0;
  }

  return h;
}

   
   buildChart() {

  const maxValue = Math.max(...this.values);
  const peakIndex = this.values.indexOf(maxValue);
  const peakLabel = this.labels[peakIndex];

  this.forecastChartOptions = {
    series: [
      {
        name: "Solar Power (kWh)",
        data: this.values
      }
    ],
    chart: {
      type: "line",
      height: 350,
      toolbar: { show: false },
      background: this.isDarkMode ? "#1e1e2f" : "#ffffff"
    },
    xaxis: {
      categories: this.labels,
      labels: {
        style: {
          colors: this.isDarkMode ? "#fff" : "#000"
        }
      }
    },
    stroke: {
      curve: "smooth",
      width: 3
    },
    markers: {
      size: 5
    },

  
    annotations: {
      points: [
        {
          x: peakLabel,
          y: maxValue,
          marker: {
            size: 8,
            fillColor: "#FF0000",
            strokeColor: "#fff"
          },
          label: {
            text: "Peak",
            style: {
              color: "#fff",
              background: "#FF0000"
            }
          }
        }
      ]
    },

    title: {
      text: "24 Hour Solar Forecast",
      style: {
        color: this.isDarkMode ? "#fff" : "#000"
      }
    },
    theme: {
      mode: this.isDarkMode ? "dark" : "light"
    }
  };
}

 dailyWeatherChart(){
  this.service.getDailyWeatherInput().subscribe((res)=>{
    if(res.status){
      this.dailyWeatherData=res.data;
      if(this.dailyWeatherData){
        console.log(this.dailyWeatherData);
        
        this.dailyWeatherLables=this.dailyWeatherData.map((x:any)=>x.date)
        this.dailyWeatherValues=this.dailyWeatherData.map((x:any)=>x.totalEnergy)
        this.buildDailyWeatherInputChart()
      }
      if(!this.dailyWeatherData){
        this.toastr.error('no data found')
      }
    }
    else{
      this.toastr.error(res.error)
    }
  })
 }
    buildDailyWeatherInputChart() {
  if (!this.dailyWeatherValues || this.dailyWeatherValues.length === 0) return;

  // Find the peak day
  const maxValue = Math.max(...this.dailyWeatherValues);
  const maxIndex = this.dailyWeatherValues.indexOf(maxValue);

  // Chart configuration
  this.dailyweatherChart = {
    series: [
      { name: "Daily Energy (KWh)", data: this.dailyWeatherValues }
    ],
    chart: {
      type: "bar",
      height: 350,
      toolbar: { show: false },
      background: this.isDarkMode ? "#000000" : "#ffffff" 
    },
    
    xaxis: {
      categories: this.dailyWeatherLables,
      labels: {
        style: {
          colors: this.isDarkMode ? "#ffffff" : "#000000"
        }
      }
    },
    yaxis: {
      title: {
        text: "Energy (KWh)",
        style: { color: this.isDarkMode ? "#ffffff" : "#000000" }
      },
      labels: {
        style: { colors: this.isDarkMode ? "#ffffff" : "#000000" }
      }
    },
    dataLabels: {
      enabled: true,
      style: { colors: this.isDarkMode ? ["#ffffff"] : ["#000000"] }
    },
    title: {
      text: "7 Day Solar Forecast",
      style: { color: this.isDarkMode ? "#ffffff" : "#000000" }
    },
    theme: {
      mode: this.isDarkMode ? "dark" : "light"
    },
    annotations: {
      yaxis: [
        {
          y: maxValue,
          borderColor: "#FF0000",
          label: {
            text: "Peak Day",
            style: { color: "#ffffff", background: "#FF0000" }
          }
        }
      ]
    }
  };
}
  
}
