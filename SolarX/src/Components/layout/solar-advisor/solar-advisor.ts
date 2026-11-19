import { Component, OnInit, OnDestroy } from '@angular/core';
import { SolarPerformanceService } from '../../../Shared/solar-performance';
import type { SolarSizing } from '../../../Models/Models';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl,FormBuilder,FormGroup,ValidationErrors,Validators,ReactiveFormsModule} from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { localMessages } from '../../../localMessages';
import { CommonModule } from '@angular/common';
import { PrimeNgModule } from '../../../app/PrimeNgModule';
import { MaterialModule } from '../../../MaterialModule';
import type { SolarSizingResponse } from '../../../Models/Models';
import { NgHttpLoaderComponent } from 'ng-http-loader';
import { delay } from 'rxjs';
import { ApexTitleSubtitle, NgApexchartsModule } from 'ng-apexcharts';
import { ApexAxisChartSeries,ApexChart,ApexDataLabels,ApexAnnotations,ApexMarkers,ApexNonAxisChartSeries,ApexFill,ApexStroke } from 'ng-apexcharts';
import{marked} from 'marked';
import DOMPurify from 'dompurify';
export type ChartOption={
 series:ApexAxisChartSeries,
 chart:ApexChart,
 xaxis:ApexXAxis,
 stroke:ApexStroke,
 dataLabels:ApexDataLabels,
 fill:ApexFill,
 tooltip:ApexTooltip,
 annotations:ApexAnnotations,
 yaxis:ApexYAxis,
 grid:ApexGrid,
 colors:string[],
 markers:ApexMarkers,
 legend:ApexLegend,
 title:ApexTitleSubtitle
}

@Component({
  selector: 'app-solar-advisor',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, MaterialModule,NgApexchartsModule],
  templateUrl: './solar-advisor.html',
  styleUrl: './solar-advisor.scss',
})
export class SolarAdvisor implements OnInit, OnDestroy {

  sizingForm!: FormGroup;
  dto!: SolarSizing;
  sizingResponse!: SolarSizingResponse;
  public chartOptions!:Partial<ChartOption>;

  destroy$ = new Subject<void>();
  messages = localMessages;
  displayed?:string;

  locations = [
    { label: 'Chennai', value: 'Chennai' },
    { label: 'Mumbai', value: 'Mumbai' },
    { label: 'Delhi', value: 'Delhi' }
  ];

  constructor(
    private build: FormBuilder,
    private toastr: ToastrService,
    private service: SolarPerformanceService
  ) {

    this.sizingForm = this.build.group(
      {
        monthlyBill: [null],
        monthlyUnits: [null],
        roofSize: [null, [Validators.required, Validators.min(22)]],
        location: [null, Validators.required],
      },
      {
        validators: this.billOrUnitsValidators
      }
    );
  }

  ngOnInit(): void {
    const billControl = this.sizingForm.get('monthlyBill');
    const unitsControl = this.sizingForm.get('monthlyUnits');

    billControl?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(val => {
      if (val) {
        unitsControl?.disable({ emitEvent: false });
      } else {
        unitsControl?.enable({ emitEvent: false });
      }
    });

    unitsControl?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(val => {
      if (val) {
        billControl?.disable({ emitEvent: false });
      } else {
        billControl?.enable({ emitEvent: false });
      }
    });
  }

  billOrUnitsValidators(group: AbstractControl): ValidationErrors | null {
    const monthlyBill = group.get('monthlyBill')?.value;
    const monthlyUnits = group.get('monthlyUnits')?.value;

    if (
      (monthlyBill == null || monthlyBill === '') &&
      (monthlyUnits == null || monthlyUnits === '')
    ) {
      return { requiredOne: true };
    }
    return null;
  }

  calculateSizing() {
    if (this.sizingForm.invalid) {
      if (this.sizingForm.errors?.['requiredOne']) {
        this.toastr.error('please fill bill or units');
      } else {
        this.toastr.error('please fill all fields');
      }
      return;
    }

    this.dto = {
      monthlyBill: this.sizingForm.value.monthlyBill,
      monthlyUnits: this.sizingForm.value.monthlyUnits,
      roofSize: this.sizingForm.value.roofSize,
      location: this.sizingForm.value.location
    };

    this.service.calculateSolarSizing(this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: async(res) => {
          if (res.status) {
            this.sizingResponse = res.data!;
            const minbattery=this.convertIntoKW(this.sizingResponse.minBatteryKWh);
            const maxbattery=this.convertIntoKW(this.sizingResponse.maxBatteryKWh);
            this.sizingResponse.minBatteryKWh=minbattery;
            this.sizingResponse.maxBatteryKWh=maxbattery;
          const fullHtml = await marked(this.sizingResponse.recommendation);
          const goodText=await DOMPurify.sanitize(fullHtml);
          await this.startTypingEffect(goodText, (val) => {
            this.displayed = val;
          });
            
            this.buildROIChart(this.sizingResponse)


            console.log('sizing response', this.sizingResponse);
          } else {
            this.toastr.error(this.messages.error.data);
          }
        },
        error: () => {
          this.toastr.error(this.messages.error.server);
        }
      });
  }

  convertIntoKW(value:number):number{
    if(value>=1000){
      value=value/1000;
    }
    return value;
  }

  startTypingEffect(fullText:string,callback:(val:string)=>void):Promise<string>{
    return new Promise((resolve)=>{
   let index = 0;

    const interval = setInterval(() => {
      callback(fullText.substring(0, index + 1));
      index++;

      if (index === fullText.length) {
        clearInterval(interval);
        resolve(fullText); 
      }
    }, 30);
    })
  }

 buildROIChart(result: any) {

  const years = Array.from({ length: 26 }, (_, i) => i);

  const yearly = result?.estimatedYearlySavings || 0;
  const systemCost = result?.estimatedSystemCost || 0;

  const savingsData = years.map(y => y * yearly);
  const costLine = years.map(() => systemCost);

  this.chartOptions = {
    series: [
      {
        name: "Savings",
        data: savingsData
      },
      {
        name: "System Cost",
        data: costLine
      }
    ],

    chart: {
      type: "area",
      height: 320,
      toolbar: { show: false },
      zoom: { enabled: false },
      background: "transparent"
    },

    title: {
      text: "Savings & ROI Analysis",
      align: "left",
      style: {
        fontSize: "16px",
        fontWeight: "600"
      }
    },

    xaxis: {
      categories: years,
      title: {
        text: "Years"
      },
      labels: {
        style: {
          colors: "#6b7280"
        }
      }
    },

    yaxis: {
      title: {
        text: "Savings (₹)"
      },
      labels: {
        formatter: (val) => `₹${(val / 1000)}k`,
        style: {
          colors: "#6b7280"
        }
      }
    },

    stroke: {
      curve: "smooth",
      width: [3, 2]
    },

    colors: ["#3b82f6", "#f59e0b"],

    fill: {
      type: ["gradient", "solid"],
      gradient: {
        shade: "light",
        type: "vertical",
        shadeIntensity: 0.5,
        opacityFrom: 0.6,
        opacityTo: 0.05,
        stops: [0, 90, 100]
      }
    },

    dataLabels: {
      enabled: false
    },

    markers: {
      size: 4,
      hover: {
        size: 6
      }
    },

    grid: {
      borderColor: "#e5e7eb",
      strokeDashArray: 4,
      xaxis: {
        lines: { show: false }
      },
      yaxis: {
        lines: { show: true }
      }
    },

    legend: {
      show: true,
      position: "top",
      horizontalAlign: "right"
    },

    tooltip: {
      shared: true,
      intersect: false,
      y: {
        formatter: (val) => `₹ ${val.toLocaleString()}`
      }
    },

    annotations: {
      xaxis: [
        {
          x: result.roiYears,
          borderColor: "#10b981",
          strokeDashArray: 4,
          label: {
            text: `ROI: ${result.roiYears} yrs`,
            style: {
              background: "#10b981",
              color: "#fff"
            }
          }
        }
      ],
      yaxis: [
        {
          y: systemCost,
          borderColor: "#f59e0b",
          strokeDashArray: 4,
          label: {
            text: "System Cost",
            style: {
              background: "#f59e0b",
              color: "#fff"
            }
          }
        }
      ]
    }
  };
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}