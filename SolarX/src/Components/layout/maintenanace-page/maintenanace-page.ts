import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { TokenService } from '../../../Shared/token';
import { localMessages } from '../../../localMessages';
import { environment } from '../../../environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RaiseRequestContent } from '../raise-request-content/raise-request-content';
import { ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-maintenanace-page',
  standalone:true,
  imports: [MaterialModule, CommonModule,FormsModule],
  templateUrl: './maintenanace-page.html',
  styleUrl: './maintenanace-page.scss',
})
export class MaintenanacePage implements OnInit {

  userId!: string;
  dto: any[] = [];
  private messages = localMessages;

  constructor(
    private toastr: ToastrService,
    private service: MaintenanceService,
    private router: Router,
    private route: ActivatedRoute,
    private token: TokenService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    const userId = this.token.GetUserId();

    if (!userId) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
    } else {
      this.userId = userId;
      this.getMaintenanceOrderForCustomer(userId);
    }
  }

  getMaintenanceOrderForCustomer(userId: string) {

    this.service.MaintenanceDataForCustomerPage(userId)
      .subscribe((res) => {

        if (res.status) {

          this.dto = (res.data ?? []).map((item: any) => ({
            ...item,
            selectedQty: 1, 
            requestedQuantity: item.requestedQuantity ?? 0
          }));

        } else {
          this.toastr.error(this.messages.error.fail);
        }

      });
  }

  getImageUrl(url?: string): string {
    if (!url) return 'Assets/Themes/panelimage1.png';
    if (url.startsWith('http')) return url;
    return environment.imageUrl + url;
  }


  isFullyRequested(item: any): boolean {
    return item.requestedQuantity >= item.quantity;
  }

  openDialogueCompoenet(item: any) {

    const dialogue = this.dialog.open(RaiseRequestContent, {
      width: '450px',
      data: {
        installationId: item.installationId,
        quantity: item.selectedQty  
      }
    });

    dialogue.afterClosed().subscribe(result => {

      if (!result) return;

      const payload = {
        installationId: item.installationId,
        userId: this.userId,
        description: result.description,
        quantity: item.selectedQty   // 👈 important
      };

      this.request(payload);
    });
  }

  request(payload: any) {

    this.service.raiseRequest(payload)
      .subscribe((res) => {

        if (res.status) {

          this.snackBar.open(
            'Maintenance request submitted successfully',
            'Close',
            {
              duration: 3000,
              horizontalPosition: 'right',
              verticalPosition: 'top',
              panelClass: ['success-snackbar']
            }
          );

          // reload data
          this.getMaintenanceOrderForCustomer(this.userId);

        } else {
          this.toastr.error(this.messages.error.fail);
        }

      });
  }

}