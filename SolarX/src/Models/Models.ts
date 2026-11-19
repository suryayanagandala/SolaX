export interface LoginResoonse{
    tokenResponse: string;
    userId: string;
    fullName: string;
    email: string;
    roleId: number;
    roleName: string;
}

export interface RegisterationCreateDto{
    fullName:string;
    phoneNumber:string;
    emailId:string;
    password:string;
    address:string;
}
export interface ResetPasswordCreateDto{
    resetToken:string;
    newPassword:string;
    confirmPassword:string;
}

export interface AdminStats {
  users: number;
  employees: number;
  orders: number;
  products: number;
  totalEmployess:number;
  panlesCount:number;
  installation:number;
  delieveries:number;

}

export interface TechnicianCreateDto {
  userId?: string;
  emailId:string;
  fullName:string;
  phoneNumber:string;
  address:string;
  specialization: number;
  discipline: number;
  qualification: number;
  experienceYears: number;
  salary: number;
  servicePrice: number;
}

export interface Product {
  productId?: number;
  productName: string;
  brand: string;
  type: any;
  capacityValue: number;
  capacity: string;
  price: number;
  warranty_Years: number;
  stock: number;
  description: string;
  createdAt?: string;   
  updateAt?: string;
  isActive?:boolean;
  deletedReason?: string | null;
  deletedAt?: string | null;
  categoryId: number; 
  categoryName?:string;
  productImages?:ProductImageResponse[]
  productReview?:ProductReviewResponse[];
  showExplaination?:boolean;
  aiText?:string;
  isImageLoaded?:boolean
}



export interface ProductImage {
  imageId?: number;
  productId: number;
  imageUrl: string;
}

export interface ApiResponse<T>{
  status:boolean,
  data?:T,
  error?:string
}

export interface ProductImageResponse{
  imageId?:number,
  imageUrl?:string,
  createdAt?:string,
  updatedAt:string,
}

export interface CategoryDto {
  categoryId: number;
  name: string;
  slug: string;
  description:string,
  deletedReason?:string;
  isActive?:boolean;
  parentId?:number,
  children?: CategoryDto[];
  productsDto?:Product[];
  categoryName?:string;
  imageUrl?:string
}

export interface CategoryForm{
  categoryId?:number;
  name:string;
  description:string;
  parentId?:number;
  image?: File;       
  imageUrl?: string; 
}

export interface ProductReviewResponse{
  reviewId?:number;
  rating:number;
  comment?:string;
  userId?:string;


}

export interface cartItemDto{
  cartItemId:number;
  productId:number;
  productName:string;
  imageUrl:string;
  price:number;
  quantity:number;
  brand:string,
  description:string,
  type:string,
  capacity:string
}

export interface CartDto{
  userId:string;
  productId?:number;
  quantity:number;
  items?:cartItemDto[]
}

export interface OrderCreateDto{
  userId:string,
  isBuyNow:boolean,
  productId?:number,
  quantity?:number,
  deliveryAddress:string,
  deliveryName:string,
  deliveryPhone:string
}

export enum Role {
  Admin = 'Admin',
  User = 'User',
  Technician='Technician'
}

export interface TechnicianDashboardDto {
  pendingCount: number;
  outForDeliveryCount: number;
  deliveredTodayCount: number;
  inProgressCount: number;
  assignedCount: number;
  completedCount: number;
  totalActiveDeliveries: number;
  specilization:string;
}

export enum TechnicianRole {
  Installer = 'Installer',
  Electrician='Electrician',
  Maintenance='Maintenance'
}
export interface MenuItem 
{ label: string;
   icon: string; 
   route: string; 
  }
export const TECHNICIAN_MENU: Record<TechnicianRole, { label: string; icon: string; route: string }[]> = 
{ [TechnicianRole.Installer]: 
  [ 
    { label: 'Dashboard', icon: 'dashboard', route: './technician-dashboard' }, 
    { label: 'Deliveries', icon: 'local_shipping', route: './deliveries' }, 
    { label: 'History', icon: 'history', route: 'history' },
    {label:'Installations',icon:'home_repair_service',route:'installations'}

  ], 
  [TechnicianRole.Maintenance]:
   [ 
    { label: 'Dashboard', icon: 'dashboard', route: 'technician-dashboard' }, 
    { label: 'Maintenance Jobs', icon: 'build', route: 'maintenanace-jobs' }, 
    { label: 'History', icon: 'history', route: 'maintainence-history' } 
  ],
   [TechnicianRole.Electrician]: 
   [ 
    { label: 'Dashboard', icon: 'dashboard', route: 'technician-dashboard' }, 
    { label: 'Maintenance Jobs', icon: 'bolt', route: 'maintenanace-jobs' }, 
    { label: 'History', icon: 'history', route: 'maintainence-history' } 
  ] 



};

export interface TrackDelivery{
  stage:string;
  status:string;
}

export interface NotifcationDto{
  userId:string;
  notificationId:number,
  notificationType:string,
  title?:string,
  createdAt:any,
  message:string,
  isRead:boolean
}

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
};


export interface RefreshTokenRequest{
  refreshToken:string;
}


export interface RecommendationResponseDto{
  accessories:Product[];
  batteries:Product[]
  inverters:Product[]
  message :string;
  aiExplanation:string;
}


export interface SolarSizing{
  monthlyBill?: number,
  monthlyUnits?: 0,
  location: string,
  roofSize: number
}

export interface SolarSizingResponse{
 requiredKW:number,
 numberOfPanels:number,
 minBatteryKWh:number,
 maxBatteryKWh:number,
 inverterKW:number,
 estimatedMonthlySavings:number,
 estimatedYearlySavings:number,
 estimatedSystemCost:number, 
 rOIYears:number, 
 recommendation:string 
}