import { ChangeDetectorRef, Component, importProvidersFrom, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationSignalrService } from '../Shared/signal-r';
import { TokenService } from '../Shared/token';
import { Notification } from '../Shared/notification';
import { NotificatoinState } from '../Shared/notificatoin-state';
import { Spinkit } from 'ng-http-loader';
import { MaterialModule } from '../MaterialModule';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MaterialModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('SolarX');
  spinkit = Spinkit
  userId!: string;
  constructor(private token: TokenService, private signalService: NotificationSignalrService,
    private notificationService: Notification, private notificationState: NotificatoinState, private cd: ChangeDetectorRef
  ) { }
  ngOnInit(): void {
    const token = this.token.getToken();
    const userId = this.token.GetUserId();
    if (token && userId) {
      this.signalService.startConnection(token);

      //set notification
      this.notificationState.loadNotifications(userId, this.notificationService);
      this.cd.detectChanges();


    }


  }
}
