import { NgModule } from '@angular/core';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
// import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
// import { SidebarModule } from 'primeng/sidebar';
// import { CalendarModule } from 'primeng/calendar';
// import { InputTextareaModule } from 'primeng/inputtextarea';
// import { TabViewModule } from 'primeng/tabview';
import { PaginatorModule } from 'primeng/paginator';
import { AutoCompleteModule } from 'primeng/autocomplete';
@NgModule({
  exports: [
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    CardModule,
    TableModule,
    DialogModule,
    // SidebarModule,
    // CalendarModule,
    // InputTextareaModule,
    // TabViewModule,
    PaginatorModule,
    AutoCompleteModule,
  ]
})
export class PrimeNgModule {}