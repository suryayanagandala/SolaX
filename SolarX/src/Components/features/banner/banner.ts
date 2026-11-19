import { Component } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-banner',
  imports: [MaterialModule, RouterLink],
  templateUrl: './banner.html',
  styleUrl: './banner.scss',
})
export class Banner {
  
}
