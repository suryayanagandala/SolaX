import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { ToastrService } from 'ngx-toastr';
import { TechnicianService } from '../../../Shared/technician-service';
import { Router } from '@angular/router';
import { TechnicianDashboardDto } from '../../../Models/Models';
import { TechnicianRole } from '../../../Models/Models';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../../localMessages';
import { finalize } from 'rxjs';
import {ApexChart, ApexNonAxisChartSeries, ApexResponsive,ApexLegend,ApexTheme} from "ng-apexcharts";
import { ChartTypeRegistry } from 'chart.js';
export type ChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  colors: string[];
  legend: ApexLegend;
  responsive: ApexResponsive[];
  theme: ApexTheme;
  stroke: ApexStroke;
  title:ApexTitleSubtitle;
  fill:ApexFill;
  plotOptions:ApexPlotOptions;
};


@Component({
  selector: 'app-technician-dashboard',
  imports: [MaterialModule,CommonModule],
  templateUrl: './technician-dashboard.html',
  styleUrl: './technician-dashboard.scss',
})
export class TechnicianDashboard implements OnInit{
  dashboard!:TechnicianDashboardDto;
  role!: TechnicianRole;
   TechnicianRole = TechnicianRole;
   isLoading:boolean=false;
   messages=localMessages;

 dashboardCards: {
  label: string;
  valueKey: keyof TechnicianDashboardDto;
  icon: string;
  roles: TechnicianRole[];
  color: string;
}[] = [
  {
    label: 'Pending',
    valueKey: 'pendingCount',
    icon: 'hourglass_empty',
    roles: [TechnicianRole.Installer, TechnicianRole.Maintenance,TechnicianRole.Electrician],
    color: 'warn'
  },
  {
    label: 'Out For Delivery',
    valueKey: 'outForDeliveryCount',
    icon: 'local_shipping',
    roles: [TechnicianRole.Installer],
    color: 'primary'
  },
  {
    label: 'Delivered Today',
    valueKey: 'deliveredTodayCount',
    icon: 'check_circle',
    roles: [TechnicianRole.Installer,TechnicianRole.Maintenance,TechnicianRole.Installer],
    color: 'accent'
  },
  {
    label: 'Total Active',
    valueKey: 'totalActiveDeliveries',
    icon: 'inventory',
    roles: [TechnicianRole.Installer, TechnicianRole.Maintenance,TechnicianRole.Electrician],
    color: 'primary'
  },
  {
    label:'Completed Count',
    valueKey: 'completedCount',
    icon: 'inventory',
    roles: [ TechnicianRole.Maintenance,TechnicianRole.Electrician],
    color: 'primary'
  },
  
];
colors=['#944148ff', '#52b789ff', '#438b9bff', '#ab913aff'];
bgColors = [
  'rgba(13,110,253,0.15)',   
  'rgba(25,135,84,0.15)',    
  'rgba(255,193,7,0.15)',    
  'rgba(13,202,240,0.15)',   
  'rgba(220,53,69,0.15)'
  
];

   public chartOptions: Partial<ChartOptions> | any;

   public monthlyOptions:Partial<ChartOptions>|any;

   public monthlyRevenue:Partial<ChartOptions>|any;

   public efficiencyOptions:Partial<ChartOptions>|any;

  assigned: number = 0;
  pending: number = 0;
  completed: number = 0;

  totalJobs: number = 0;
  completedJobs: number = 0;

  constructor(private service:TechnicianService,private toastr:ToastrService,
    private router:Router
  ){}

  ngOnInit(): void {
   this.getDashboardDat(); 
   this.getWorkStatus();
   this.monthlyChart();
   this.getRevenue();
   this.getEfficiency();
  }

   getTheme() {
    return document.body.classList.contains('dark-theme') ? 'dark' : 'light';
  }

  getDashboardDat(){
    this.isLoading=true
    this.service.TechinicianDashbaord().pipe(finalize(()=>{this.isLoading=false})).subscribe((res)=>{
      if(res.status){
        this.dashboard=res.data??{};
        this.role=this.dashboard.specilization as TechnicianRole
        
      }
      else{
        this.toastr.error(this.messages.error.fail)
      }
    },
  )
  }

  isCardVisible(card:any):boolean{
 return card.roles.includes(this.role)
  }

   getWorkStatus() {

    this.service.getWorkStats().subscribe(res => {
         if(!res?.data){
      return;
    }        this.assigned = res.data.assigned;
        this.pending = res.data.pending;
        this.completed = res.data.completed;
         const themeMode = this.getTheme();
        this.chartOptions = {
          series: [
            this.assigned,
            this.pending,
            this.completed
          ],
          chart: {
            type: "donut",
            height: 350
          },
          labels: [
            "Assigned Jobs",
            "Pending Jobs",
            "Completed Jobs"
          ],
          colors: [
            "#ef4444",
            "#f59e0b",
            "#22c55e"
          ],
          legend: {
            position: "bottom"
          },
          responsive: [
            {
              breakpoint: 480,
              options: {
                chart: {
                  width: 300
                }
              }
            }
          ]
        };

      });

  }

  monthlyChart(){
    this.service.getMonthlyUpdates().subscribe((res)=>{
      if(res.status){
         const months: string[] = [];
        const jobs: number[] = [];

        res.data.forEach((item: any) => {
          months.push(item.month);
          jobs.push(item.completedJobs);
        });
         const themeMode = this.getTheme(); 
         this.monthlyOptions = {
          series: [
            {
              name: "Completed Jobs",
              data: jobs
            }
          ],
          chart: {
            type: "bar",
            height: 350
          },
          title: {
            text: "Monthly Completed Jobs"
          },
          xaxis: {
            categories: months
          },
          colors: themeMode === 'dark'
            ? ['#4ade80']  
            : ['#16a34a'],  
          theme: {
            mode: themeMode
          } 
        };
      }
    })
  }

   getRevenue() {

    this.service.getEmployeeRevenue().subscribe((res) => {

      if(!res.status){
        return;
      }

        const months: string[] = [];
        const revenue: number[] = [];

        res.data.forEach((item: any) => {
          months.push(item.month);
          revenue.push(item.revenue);
        });

        const themeMode = this.getTheme();

        const primaryColor = themeMode === 'dark'
          ? '#60a5fa'
          : '#2563eb';

        this.monthlyRevenue = {
          series: [
            {
              name: "Revenue",
              data: revenue
            }
          ],
          chart: {
            type: "line",
            height: 350
          },
          title: {
            text: "Monthly Revenue"
          },
          xaxis: {
            categories: months
          },
          stroke: {
            curve: "smooth",
            width: 3
          },
          colors: [primaryColor],
          theme: {
            mode: themeMode
          },
          fill: {
            type: "gradient",
            gradient: {
              shadeIntensity: 1,
              type: "vertical",
              opacityFrom: 0.6,
              opacityTo: 0.1,
              stops: [0, 90, 100]
            }
          }
        };

      });

  }

  getEfficiency(){
    this.service.getEmployeeEfficinecy().subscribe((res)=>{
      if(res.status){

        const efficiency = Math.round(res.data.efficiency);
        this.totalJobs = res.data.totalJobs;
        this.completedJobs = res.data.completedJobs;

        const themeMode = this.getTheme();

        const gaugeColor = efficiency >= 80
          ? '#22c55e'  : efficiency >= 50? '#f59e0b'  : '#ef4444';  

        this.efficiencyOptions = {
          series: [efficiency],
          chart: {
            height: 320,
            type: "radialBar"
          },
          plotOptions: {
            radialBar: {
              hollow: {
                size: "65%"
              },
              dataLabels: {
                name: {
                  show: true,
                  fontSize: "16px"
                },
                value: {
                  show: true,
                  fontSize: "28px",
                  formatter: function (val: number) {
                    return val + "%";
                  }
                }
              }
            }
          },
          colors: [gaugeColor],
          labels: ["Efficiency"],
          theme: {
            mode: themeMode
          }
        };

      }
    })
  }

}
