import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter,withInMemoryScrolling } from '@angular/router';
import { provideHttpClient,withInterceptors } from '@angular/common/http';
import { authInterceptor } from './Interceptor/auth.interceptor';
import { routes } from './app.routes';
import { provideAnimations} from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { NgApexchartsModule } from "ng-apexcharts";
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { providePrimeNG } from 'primeng/config';
// import { NgHttpLoaderModule } from 'ng-http-loader';
import { withInterceptorsFromDi } from '@angular/common/http';
import { NgHttpLoaderModule } from 'ng-http-loader';
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes,withInMemoryScrolling({anchorScrolling:'enabled',scrollPositionRestoration:'enabled'})),
    provideHttpClient(withInterceptors([authInterceptor]),withInterceptorsFromDi()),
    provideToastr({
      positionClass:'toast-bottom-right'
    }),provideAnimations(),importProvidersFrom(NgApexchartsModule),
    provideCharts(withDefaultRegisterables()),
    providePrimeNG({
     ripple: true
   }),
   importProvidersFrom(NgHttpLoaderModule.forRoot())
  ]
};
