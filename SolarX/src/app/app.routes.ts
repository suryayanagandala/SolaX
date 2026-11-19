import { Routes } from '@angular/router';
import { LoginComponent } from '../Components/login-component/login-component';
import { RegisterComponent } from '../Components/register-component/register';
import { ForgetPassword } from '../Components/forget-password/forget-password';
import { VerifyOtp } from '../Components/verify-otp/verify-otp';
import { ResetPassword } from '../Components/reset-password/reset-password';
import { AdminPanel } from '../Components/AdminComponents/admin-panel/admin-panel';
import { Appmenu } from '../Components/appmenu/appmenu';
import { authGuard } from './Guards/auth.guard';
import { AdminUsers } from '../Components/AdminComponents/admin-users/admin-users';
import { AdminDashboard } from '../Components/AdminComponents/admin-dashboard/admin-dashboard';
import { AssignTechnicians } from '../Components/AdminComponents/assign-technicians/assign-technicians';
import { AddProducts } from '../Components/AdminComponents/add-products/add-products';
import { ProductForm } from '../Components/AdminComponents/product-form/product-form';
import { ViewImages } from '../Components/AdminComponents/view-images/view-images';
import { ImagesForm } from '../Components/AdminComponents/images-form/images-form';
import { App } from './app';
import { Banner } from '../Components/features/banner/banner';
import { Home } from '../Components/features/home/home';
import { loginGaurd } from './Guards/login.guard';
import { technicianGuard } from './Guards/Technician.Guard';

export const routes: Routes = [
  //  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgotPassword', component: ForgetPassword },
  { path: 'verify-otp', component: VerifyOtp },
  { path: 'resetpassword', component: ResetPassword },

   {
    path: '',
    component: Appmenu, // Header + Footer
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },

      {
        path: 'home',
        loadComponent: () =>
          import('../Components/features/home/home').then(m => m.Home),
      },
      {
        path: 'products',
        loadComponent: () =>
          import('../Components/features/products/products').then(m => m.ProductsHome),
      },
      {
        path:'view-deatils',
        loadComponent:()=>
          import('../Components/features/view-product-details/view-product-details').then(x=>x.ViewProductDetails)
      },
      
      {
        path:'track-order',canActivate:[loginGaurd],
        loadComponent:()=>import('../Components/layout/track-order/track-order').then(x=>x.TrackOrder)
      },
      {
         path: 'dashboard',
         canActivate: [loginGaurd],
         loadComponent: () =>import('../Components/layout/dashboard/dashboard').then(m => m.Dashboard),
       },
       {
         path:'order',
         canActivate: [loginGaurd],
         loadComponent:()=>import('../Components/layout/orders/orders').then(x=>x.Orders)
       },{
         path:'cart',
         canActivate: [loginGaurd],
        loadComponent:()=>import('../Components/layout/cart/cart').then(x=>x.Cart)
     },

     {
       path:'payment',
       canActivate:[loginGaurd],
       loadComponent:()=>import('../Components/layout/payment-component/payment-component').then(x=>x.PaymentComponent)
     },
     {
        path:'payment-success',
        canActivate:[loginGaurd],
        loadComponent:()=>import('../Components/layout/payment-sucess/payment-sucess').then(x=>x.PaymentSucess)
     },
     {
       path:'order-history',
       canActivate:[loginGaurd],
       loadComponent:()=>import('../Components/layout/order-history/order-history').then(x=>x.OrderHistory)
     },
     {
       path:'maintenance-page',
       canActivate:[loginGaurd],
       loadComponent:()=>import('../Components/layout/maintenanace-page/maintenanace-page').then(x=>x.MaintenanacePage)
     },
      
      {
        path: 'login',
        loadComponent: () =>
          import('../Components/login-component/login-component').then(m => m.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('../Components/register-component/register').then(m => m.RegisterComponent),
      },
      {
        path:'solar-advisor',
        loadComponent:()=>import('../Components/layout/solar-advisor/solar-advisor').then(x=>x.SolarAdvisor)
      }
      
    ],
  },

{
        path:'notification',
        loadComponent:()=>
         import('../Components/notifications/notifications').then(x=>x.Notifications)
},

 


  {
    path: 'admin',
    canActivate: [authGuard],
    loadComponent: () =>
      import('../Components/AdminComponents/admin-panel/admin-panel')
        .then(x => x.AdminPanel),
    children: [
      { path: '', redirectTo: 'admin-dashboard', pathMatch: 'full' },
      {
        path: 'admin-dashboard',
        loadComponent: () =>
          import('../Components/AdminComponents/admin-dashboard/admin-dashboard')
            .then(x => x.AdminDashboard)
      },
      {
        path: 'admin-users',
        loadComponent: () =>
          import('../Components/AdminComponents/admin-users/admin-users')
            .then(x => x.AdminUsers)
      },
      {
        path: 'assign-technician',
        loadComponent: () =>
          import('../Components/AdminComponents/assign-technicians/assign-technicians')
            .then(x => x.AssignTechnicians)
      }, {
        path: 'product-details',
        loadComponent: () => import('../Components/AdminComponents/add-products/add-products').then(x => x.AddProducts)
      },
      {
        path: 'productOperations',
        loadComponent: () => import('../Components/AdminComponents/product-form/product-form').then(x => x.ProductForm)
      },
      {
        path: 'viewImages',
        loadComponent: () => import('../Components/AdminComponents/view-images/view-images').then(x => x.ViewImages)
      },
      {
        path: 'images-form',
        loadComponent: () => import('../Components/AdminComponents/images-form/images-form').then(x => x.ImagesForm)
      },
      {
        path: 'category-list',
        loadComponent: () => import('../Components/AdminComponents/category-list/category-list').then(x => x.CategoryList)
      },
      {
        path:'category-form',
        loadComponent:()=>import('../Components/AdminComponents/category-form/category-form').then(x=>x.CategoryFormComponent)
      },
      
    ]
  },


  {
    path: 'dashboard', canActivate: [authGuard],
    loadComponent: () => import('../Components/appmenu/appmenu').then(x => x.Appmenu)
  },

  {path:'products',loadComponent:()=>import('../Components/features/products/products').then(x=>x.ProductsHome)},
  
  {path:'panel',canActivate:[technicianGuard],
    loadComponent:()=>import('../Components/TechnicianComponents/technician-panel/technician-panel')
    .then(x=>x.TechnicianPanel),
    children:[
      {path:'',redirectTo:'technician-dashboard',pathMatch:'full'},
      {
        path:'technician-dashboard',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/technician-dashboard/technician-dashboard').then(x=>x.TechnicianDashboard)
      },
      {
        path:'deliveries',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/deliveries/deliveries').then(x=>x.Deliveries)
      },
      {
        path:'history',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/history/history').then(x=>x.History)
      },
      {
        path:'installations',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/installations/installations').then(x=>x.Installations)
      },
      {
        path:'report-form',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/report-form/report-form').then(x=>x.ReportForm)
      },
      {
        path:'maintenanace-jobs',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/maintenance-job/maintenance-job').then(x=>x.MaintenanceJob)

      },
      {
        path:'maintenance-form',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/maintenance-report/maintenance-report').then(x=>x.MaintenanceReport)
      },
      {
        path:'maintainence-history',canActivate:[technicianGuard],
        loadComponent:()=>import('../Components/TechnicianComponents/maintenance-history/maintenance-history').then(x=>x.MaintenanceHistory)
      }
    ]
  },
  {
    path:'model-1',
    loadComponent:()=>import('../Components/Seperate/model-1/model-1').then(x=>x.Model1)
  },
  {
    path:'model-2',
    loadComponent:()=>import('../Components/Seperate/model-2/model-2').then(x=>x.Model2)
  },
  {
    path:'pricing-model',
    loadComponent:()=>import('../Components/Seperate/pricing-model/pricing-model').then(x=>x.PricingModel)
  }
    
];
