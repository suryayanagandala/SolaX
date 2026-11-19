import { NgModule } from "@angular/core";
import { MatCardModule } from "@angular/material/card";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatSortModule } from "@angular/material/sort";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatIconModule } from "@angular/material/icon";
import { MatDialogModule } from "@angular/material/dialog";
import { MatMenuModule } from "@angular/material/menu";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatListModule } from "@angular/material/list";
import { MatFormFieldControl } from "@angular/material/form-field";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { MatBadgeModule } from '@angular/material/badge';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { MatGridListModule } from '@angular/material/grid-list';
import { NgHttpLoaderModule } from "ng-http-loader";
import { MatStepperModule } from '@angular/material/stepper';
import { MatChipsModule } from '@angular/material/chips';
import { ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
@NgModule({
  imports: [
    BrowserAnimationsModule
  ]
})
export class AppModule { }
@NgModule({
  exports: [
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatCheckboxModule,
    MatSortModule, MatPaginatorModule, MatIconModule, MatDialogModule, MatMenuModule, MatToolbarModule,
    MatSidenavModule, MatListModule, MatProgressSpinnerModule, MatExpansionModule, MatTabsModule,
    MatBadgeModule, MatSnackBarModule, NgApexchartsModule, MatDatepickerModule, MatNativeDateModule,
    MatGridListModule, NgHttpLoaderModule,MatStepperModule,MatChipsModule,ReactiveFormsModule,CommonModule,
    FormsModule
  ]
})
export class MaterialModule {

}