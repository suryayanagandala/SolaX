import { AfterViewInit, Component, OnInit } from '@angular/core';
import { materialize,Subscription,interval } from 'rxjs';
import { MaterialModule } from '../../../MaterialModule';
import { AdminStats } from '../../../Models/Models';
import { Auth } from '../../../Shared/auth';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { localMessages } from '../../../localMessages';
import {ApexAxisChartSeries,ApexChart,ApexXAxis,ApexYAxis,ApexStroke,ApexDataLabels,
  ApexTitleSubtitle,ApexTooltip,ApexGrid, ApexTheme,ApexMarkers} from "ng-apexcharts";
import { NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { BaseChartDirective } from 'ng2-charts';
import { ViewChild } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import { ChangeDetectorRef } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
export type ChartOption = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  stroke: ApexStroke;
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  grid: ApexGrid;
  theme: ApexTheme;
};

@Component({
  selector: 'app-admin-dashboard',
  
  imports: [MaterialModule,NgApexchartsModule,CommonModule,BaseChartDirective,MatProgressBarModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard implements OnInit,AfterViewInit{
  stats!:AdminStats;
  productionData!:any;
  monthlyChart!:ChartOption;
  dailyChart!:ChartOption;
  growthPercent!:number;
  alerts!:any;
  refreshSub!:Subscription;
  private messages=localMessages;
  productTrends!:any;
  soldProducts!:any[];
  aiProductUpdates!:any;
  anmalyData!:any[];
  cd!:ChangeDetectorRef;
  aiAlerts: any[] = [];
  @ViewChild('productTrendChart') productTrendChart?: BaseChartDirective;
@ViewChild('soldProductsChart') soldProductsChart?: BaseChartDirective;
  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        label: 'Sold Quantity',
        data: [],
        backgroundColor: '#6d7173'
      }
    ]
  };
  isDark = document.body.classList.contains('dark-theme');
 barChartOptions: ChartConfiguration<'bar'>['options'] = {
  responsive: true,
  indexAxis: 'y', 

  plugins: {
    legend: {
      display: true
    }
  },

  scales: {
    x: {
      beginAtZero: true,
      ticks: {
       color: this.isDark ? '#ffffff' : '#333333'
      }
    },
    y: {
      ticks: {
        color: this.isDark ? '#ffffff' : '#333333'
      }
    }
  }
};

trendChartData: ChartConfiguration<'bar'>['data'] = {
  labels: [],
  datasets: [
    {
      label: 'Growth %',
      data: []
    }
  ]
};

trendChartOptions: ChartConfiguration<'bar'>['options'] = {
  indexAxis: 'y',   
  responsive: true,
  plugins: {
    legend: {
      display: false
    }
  }
};

panelHealthChart: ChartConfiguration<'doughnut'>['data'] = {
  labels: ['Offline Panels', 'Low Production Panels'],
  datasets: [
    {
      data: []
    }
  ]
};

panelHealthOptions: ChartConfiguration<'doughnut'>['options'] = {
   responsive:true,
  cutout:'70%',
  plugins:{
    legend:{
      position:'bottom'
    }
  }
};

lowStockChart: ChartConfiguration<'bar'>['data'] = {
  labels: [],
  datasets: [
    {
      label: 'Stock Remaining',
      data: []
    }
  ]
};

lowStockOptions: ChartConfiguration<'bar'>['options'] = {
  indexAxis: 'y',
  responsive: true
};




  constructor(private service:Auth,private toastr:ToastrService,private router:Router){}
  
ngAfterViewInit() {
  setTimeout(() => {
    this.updateChartTheme();
  }, 200);
}



  ngOnInit(): void {
    this.LoadStats();
    this.getGlobalData();
    this.getProductTrend();
    this.getSoldProducts();
    this.getProductionUpdate();
    this.updateChartTheme();
    // this.refreshSub=interval(5000).subscribe(()=>{
    //   this.getSoldProducts();
    //   this.getGlobalData()
    // })
    this.getSoldProducts();
      this.getGlobalData()

    this.getProductUpdatesWithAi();
      
  }
  LoadStats(){
    this.service.getStats().subscribe({
      next:(res)=>{
        if(res.status){
          this.stats=res.data;
          console.log(this.stats);
          
        }
        else{
          console.log(res.error);
          this.toastr.error(this.messages.error.data);
        }
      },
      error(err) {
        console.log(err);
        
      },
    })
  }

 
  getGlobalData()
  {
    this.service.getProductionData().subscribe({
      next:res=>{
        if(res.status){
         this.productionData=res.data;
         console.log('productionData',this.productionData);
         
         this.growthPercent=this.productionData.changePercent;
         this.updateMonthlyChart(this.productionData.monthlyTrend)
         this.updateDailyTrend(this.productionData.dailyTrend)
        }
      }
    })
  }

updateMonthlyChart(data: any[]) {

  const isDark = document.body.classList.contains('dark-theme');

  const months = [
    'Jan','Feb','Mar','Apr','May','Jun',
    'Jul','Aug','Sep','Oct','Nov','Dec'
  ];

  const values = new Array(12).fill(0);

  data?.forEach(x => {
    if (x.month >= 1 && x.month <= 12) {
      values[x.month - 1] = (x.totalPower ?? 0) / 1_000_000; 
    }
  });

  this.monthlyChart = {
    series: [{
      name: "Total Production (MW)",
      data: values
    }],
    chart: {
      type: "bar",
      height: 350,
      toolbar: { show: false },
      background: 'transparent'
    },
    xaxis: {
      categories: months,
      labels: {
        style: {
          colors: isDark ? '#ffffff' : '#333333'
        }
      }
    },
    yaxis: {
      labels: {
        formatter: (val) => val.toFixed(2) + " MW",
        style: {
          colors: isDark ? '#ffffff' : '#333333'
        }
      }
    },
    theme: {
      mode: isDark ? 'dark' : 'light'
    },
    stroke: {
      curve: 'smooth',
      width: 3
    },
    dataLabels: {
      enabled: true,
      formatter: (val: number) => val.toFixed(2) + " MW",
      style: {
        colors: [isDark ? '#ffffff' : '#000000']
      }
    },
    grid: {
      borderColor: isDark ? '#444' : '#e0e0e0'
    },
    title: {
      text: "Monthly Production",
      style: {
        color: isDark ? '#ffffff' : '#000000'
      }
    }
  };
}

updateDailyTrend(data: any[]) {

  const isDark = document.body.classList.contains('dark-theme');

  const hours = Array.from({ length: 24 }, (_, i) => i + ":00");
  const values = new Array(24).fill(0);

  data?.forEach(x => {
    if (x.hour >= 0 && x.hour <= 23) {
      values[x.hour] = (x.totalPower ?? 0) / 1000; 
    }
  });

  this.dailyChart = {
    series: [{
      name: "Daily Production (kW)",
      data: values,
      
    }],
    chart: {
      type: "line",
      height: 350,
      toolbar: { show: false },
      background: 'transparent'
    },
    xaxis: {
      categories: hours,
      labels: {
        style: {
          colors: isDark ? '#ffffff' : '#333333'
        }
      }
    },
    yaxis: {
      labels: {
        formatter: (val) => val.toFixed(1) + " kW",
        style: {
          colors: isDark ? '#ffffff' : '#333333'
        }
      }
    },
    theme: {
      mode: isDark ? 'dark' : 'light'
    },
    stroke: {
      curve: 'smooth',
      width: 3
    },
    dataLabels: {
      enabled: true,
      formatter: (val: number) => val.toFixed(1) + " kW",
      style: {
        colors: [isDark ? '#ffffff' : '#000000']
      }
    },
    grid: {
      borderColor: isDark ? '#444' : '#e0e0e0'
    },
    title: {
      text: "Daily Production",
      style: {
        color: isDark ? '#ffffff' : '#000000'
      }
    }
  };
}



getProductTrend(){
  this.service.getProductionTrend().subscribe((res)=>{
    if(res.status){

      this.productTrends = res.data ?? [];

      this.trendChartData = {
        labels: this.productTrends.map((x:any) => x.productName),
        datasets: [
          {
            label: 'Growth %',
            data: this.productTrends.map((x: any) => x.changePercentage)
          }
        ]
      };

    }
  })
}


    getSoldProducts() {
      this.service.getSoldProductsData().subscribe((res) => {
        if (res.status) {

          this.soldProducts = res.data[0] ?? [];

          this.barChartData = {
            labels: this.soldProducts.map(x => x.productName),
            datasets: [
              {
                label: 'Sold Quantity',
                data: this.soldProducts.map(x => x.quantity),
                backgroundColor: '#42A5F5'
              }
            ]
          };

          // this.soldProductsChart?.update();
        }
      });
    }

getProductionUpdate(){
  this.service.getProductionUpdate().subscribe((res)=>{
    if(res.status){

      this.alerts = res.data;

      this.panelHealthChart = {
        labels: ['Offline Panels','Low Production Panels'],
        datasets: [
          {
            data: [
              this.alerts.offlinePanels,
              this.alerts.lowProductionPanels
            ]
          }
        ]
      };

      this.lowStockChart = {
  labels: this.alerts.lowProductStock.map((x:any) => x.productName),
  datasets: [
    {
      label: 'Stock Remaining',
      data: this.alerts.lowProductStock.map((x:any) => x.stock)
    }
  ]
};

    }
  })
}

getImageUrl(url?: string): string {
      if (!url) {
        return 'Assets/Themes/panelimage1.png';
      }
  
      if (url.startsWith('http')) {
        return url;
      }
  
      return environment.imageUrl + url;
    }

   updateChartTheme() {

  const isDark = document.body.classList.contains('dark-theme');

  const textColor = isDark ? '#ffffff' : '#333333';
  const gridColor = isDark ? '#444' : '#e0e0e0';

  
  if (this.productTrendChart?.chart) {

    this.productTrendChart.chart.options.plugins!.legend!.labels!.color = textColor;

    this.productTrendChart.chart.options.scales!['x']!.ticks!.color = textColor;
    this.productTrendChart.chart.options.scales!['y']!.ticks!.color = textColor;

    this.productTrendChart.chart.options.scales!['x']!.grid!.color = gridColor;
    this.productTrendChart.chart.options.scales!['y']!.grid!.color = gridColor;

    this.productTrendChart.chart.update();
  }

 
  if (this.soldProductsChart?.chart) {

    this.soldProductsChart.chart.options.plugins!.legend!.labels!.color = textColor;

    this.soldProductsChart.chart.options.scales!['x']!.ticks!.color = textColor;
    this.soldProductsChart.chart.options.scales!['y']!.ticks!.color = textColor;

    this.soldProductsChart.chart.options.scales!['x']!.grid!.color = gridColor;
    this.soldProductsChart.chart.options.scales!['y']!.grid!.color = gridColor;

    this.soldProductsChart.chart.update();
  }

}
 getProductUpdatesWithAi(){
  this.service.getProductionUpdateWithAI().subscribe((res)=>{
    if(res.status){
      this.aiProductUpdates=res.data;
      if(this.aiProductUpdates.anomalyDetails.length){
        this.anmalyData=this.aiProductUpdates.anomalyDetails;
      }
      console.log('Ai Data',this.aiProductUpdates);
      
    }
  })
 }

}
