    --create database----
    create database SolarProject;

    --use data base----
    use [SolarProject];

    ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;	-- To Prevent from Identity Cache 

    ---create tables-------

    /* Role */
       CREATE TABLE Roles (
        RoleId INT IDENTITY(1,1),
        RoleName VARCHAR(50) NOT NULL,
        IsActive BIT NOT NULL,
        CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
        CONSTRAINT PK_RoleId PRIMARY KEY (RoleId),
        CONSTRAINT UK_RoleName UNIQUE (RoleName),
        );

        --Alter table---
    Alter table Roles Add Constraint Df_IsActive DEFAULT 1 for IsActive;
     --sample data insertion---
     insert into Roles(RoleName) 
     values('Customer');

     --update role table---
     update Roles set RoleName='Technician' where RoleId=2;

     ---get table---
     select * from Roles;

     /* Users */
     CREATE TABLE Users(
      UserId varchar(6) not null,
      FullName nvarchar(200) not null,
      EmailId nvarchar(250) not null,
      PasswordHash nvarchar(500) not null,
      PhoneNumber Nvarchar(20) not null,
      RoleId int not null default 3,
      IsActive bit not null default 1,
      CreatedAt datetimeoffset(7) not null Constraint Df_CreatedAt Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),

      Constraint Pk_UserId primary key(UserId),
      Constraint Uk_FullName Unique(FullName),
      Constraint Uk_EmailId unique(EmailId),
      Constraint Fk_RoleId foreign key(RoleId) references Roles(RoleId),
     );


     --Alter table for adding constraint on users table---
     Alter table Users Add Constraint Ch_PhoneNumber Check(Len(PhoneNumber)=10 and PhoneNumber not like '%[^0-9]%')
     Alter table Users Alter Column PhoneNumber nvarchar(15) not null;
     Alter table Users Add Constraint CHK_Email_Valid Check(EmailId LIKE '%_@_%.__%' AND EmailId NOT LIKE '%[^a-zA-Z0-9@._-]%');
     Alter table Users Drop Constraint Ch_PhoneNumber;
      update Users set PhoneNumber='919666308380' where userId='Z3HXXB';
     Alter Table Users Add  UpdatedBy nvarchar(100);
     Alter table Users Add UpdatedAt DateTimeOffSet(7);
 
     Alter table Users Add CreatedBy nvarchar(200) not null Constraint Df_CreatedBy Default 'Admin';

     Alter table Users alter column EmailId varchar(250);

     Alter Table Users Add  Address varchar(350);

     Alter Table Users Alter column Address Varchar(350) not null;
   
 
     --Indexs---
     Create Index Ind_UserId on Users(UserId);
     Create Index Ind_Email on Users(EmailId);

     ---Insert sample data into users--
    Insert into Users (UserId, FullName, EmailId, PasswordHash, PhoneNumber, RoleId, IsActive)values
    ('USR001', 'Ravi Kumar', 'ravi@example.com', 'hash1', '9876543210', 3, 1),
    ('USR002', 'Priya Sharma', 'priya@example.com', 'hash2', '9123456780', 3, 1),
    ('USR003', 'Arun Verma', 'arun@example.com', 'hash3', '9988776655', 3, 1);

     --get the data from Users---
     select * from Users;

     --Checking the user with role ----
     select f.FullName,f.EmailId,f.RoleId,f.UserId,r.RoleName,f.CreatedAt as JoinedDate from Users f 
     inner join Roles r on r.RoleId=f.RoleId

     /*Technicians*/
     Create Table Technicians(
     TechnicianId int identity(100,1) not null,
     UserId varchar(6) not null,
     Specialization nvarchar(50) not null,
     ExperienceYears decimal(2,1) not null,
     Qualification nvarchar(100) not null,
     Descipline nvarchar(100) ,
     Salary decimal(12,2) not null,
     Service_Price decimal(10,2) not null,
     IsActive bit not null Constraint If_Isactive Default 1,

     Constraint Pk_TechId Primary Key(TechnicianId),
     Constraint Fk_UserId Foreign Key(UserId) references Users(UserId),
     Constraint Chk_Specilization Check( Specialization IN ('Installer', 'Maintenance', 'Electrician','Supervisior')),
     
     Constraint Chk_Salary Check(Salary>0),
     Constraint Chk_Service_Price Check(Service_Price>100)
     );

     Alter table Technicians drop constraint Chk_Experience;

    ALTER TABLE Technicians ADD CONSTRAINT CK_Technicians_ExperienceYears CHECK (ExperienceYears >= 0);
     --Insert into table---
     insert into Technicians(UserId,Specialization,ExperienceYears,Qualification,Descipline,Salary,Service_Price)
     values('TCH123','Installer','1','Diploma','Mechanical',35000,250);

     Alter Table Technicians Add IsAvailable bit default 1 not null;
     Alter table Technicians Add IsActiveDeliveries int null;
     Alter table Technicians Alter Column IsActiveDeliveries int  not null;
  
     Update Technicians set IsActiveDeliveries=0 where Specialization='Installer';

    
     --get data from technicians table----
     select * from Technicians;

     ---fetch the data udin joins---
     select u.UserId,u.FullName,r.RoleName,t.ExperienceYears,t.Salary from Users u inner join 
     Technicians t on t.UserId=u.UserId inner join Roles r 
     on r.RoleId=u.RoleId where r.RoleId=2;

     select * from Users where userId='W2RGRR';
     /*Products*/
     Create Table Products(
     ProductId int not null identity(1000,1),
     ProductName varchar(250) not null,
     Brand varchar(200) not null,
     Type varchar(100) not null,
     CapacityValue int not null,
     Capacity nvarchar(10) not null,
     Price decimal(15,2) not null,
     Warranty_Years decimal(5,1) not null,
     Stock int not null Constraint df_Stcok default 0,
     Description nvarchar(500) not null Constraint Chk_description check(Len(Description)>30),
     IsActive Bit not null Constraint Df_Active default 1,
     CreatedAt datetimeoffset(7) not null Constraint Df_Created Default switchoffset(SysDateTimeOffSet(),'+05:30'),

     Constraint Pk_ProductId Primary Key(ProductId),
     Constraint Uk_Name Unique(ProductName),
     Constraint Chk_Type Check(Type in('Mono','Poly','PERC','TOPCon','HJT','TFSP','Bifacial')),
     Constraint Chk_Capacity Check(CapacityValue>0),
     Constraint Chk_Price Check(Price>0),
     Constraint Chk_Warranty Check(Warranty_Years>0),

     );

     
     --Alter table fo adding UodatedAt---
     Alter table Products Add UpdatedAt datetimeOffSet(7);

     Alter table Products Add CategoryId int foreign key references [dbo].[Categories]([CategoryId]);

     Alter table Products drop Constraint 

   
     ALTER TABLE Products DROP CONSTRAINT Chk_description;
     Alter table Products Add AverageRating float;

     Alter table Products Add DeletedReason varchar(500);

     Alter table Products Add DeletedAt DateTimeOffSet(7);
     ALTER TABLE Products ADD CONSTRAINT Chk_description CHECK(LEN(Description) > 10)
      Alter table Products Alter column Capacity nvarchar(30);

     --Insert into Products Table---
     Insert into Products 
    (ProductName, Brand, Type, CapacityValue, Capacity, Price, Warranty_Years, Stock, Description)values
    ('Adani Mono 550W Solar Panel', 'Adani', 'Mono', 550, '550W', 11500.00, 10, 50,
     'High-efficiency mono PERC solar panel suitable for rooftop and commercial installations.'),

    ('Vikram Poly 330W Solar Panel', 'Vikram Solar', 'Poly', 330, '330W', 7800.00, 5, 120,
     'Reliable polycrystalline solar module with strong performance even in low-light conditions.'),

    ('Waaree TOPCon 600W Bifacial Panel', 'Waaree', 'TOPCon', 600, '600W', 14500.00, 12, 30,
     'Advanced TOPCon bifacial module providing superior efficiency and performance in all climates.');

     --get data from products---
     select * from products;
     
     /*ProductImage*/
     Create Table ProductImage(
     ImageId int not null identity(100,1),
     ProductId int not null,
     ImageUrl nvarchar(1000) not null,
     CreatedAt Datetimeoffset(7) not null constraint Df_ProductImage_CreatedAt Default switchoffset(SysDateTimeOffSet(),'+05:30'),
     UpdatedAt DateTimeOffSet(7),
     IsActive bit not null Constraint Df_ProductImage_Active Default 1,

     Constraint Pk_ImageId Primary Key(ImageId),
     Constraint Fk_ProductId Foreign Key(ProductId) References Products(ProductId)

     );

     --insert into productImage table---
     Insert into ProductImage (ProductId, ImageUrl)values
    (1000, 'https://example.com/images/mono_550w_front.jpg'),
    (1000, 'https://example.com/images/mono_550w_side.jpg'),
    (1001, 'https://example.com/images/poly_330w_front.jpg');

    --fetch data---
    select i.ProductId,p.ProductName,p.Price,P.Capacity,i.ImageUrl from  ProductImage i
    inner join Products p on p.ProductId=i.ProductId where p.ProductId=1000;

    /*Cart Table*/
    Create Table CartItems(
      CartItemId int not null identity(100,1),
      CartId int  not null,
      ProductId int not null,
      Quantity int not null Constraint Df_Cart_Quantity Default 1,
      Price decimal(12,2) not null,
      IsActive bit not null Default 1,
  
      Constraint Pk_Cart_CartId Primary Key (CartItemId),
      Constraint Fk_CartId Foreign Key(CartId) References Cart(CartId),
      Constraint Fk_Cart_ProductId Foreign Key(ProductId) References Products(ProductId)
    );

    Alter table CartItems Add UpdatedAt DateTimeOffSet(7);


   
   ----Cart Table-----
    Create table Cart(
    CartId int identity(100,1) constraint Pk_CartId Primary key,
    UserId varchar(6) not null Constraint Fk_Cart_UserId Foreign key References Users(UserId),
    CreatedAt DateTimeOffSet(7) not null  Constraint Df_Cart_CreatedAt Default 
    SwitchOffSet(SysDateTimeOffSet(),'+05:30'));

   Alter table Cart Add IsActive bit not null default 0;

    --insert into Cart Tbale---
    Insert into Cart (UserId, ProductId, Quantity) values
    ('USR001', 1000, 1),
    ('USR001', 1001, 2),
    ('USR002', 1002, 1),
    ('USR003', 1000, 3);

    --get Cart Data---
    select * from Cart;

    /*Orders*/
    Create Table Orders(
    OrderId int not null Identity(100,1) Constraint Pk_Orders_OrderId Primary Key,
    UserId varchar(6) not null,
    OrderDate Date not null Constraint Df_Orders_OrderDate Default GetDate(),
    Status varchar(50) not null,
    TotalAmount decimal(14,2) NOT NULL Default 0.00,
    DeliveryAddress nvarchar(1000) not null,
    IsActive bit not null default 1,
    CreatedAt DateTimeOffSet(7) not null Constraint Df_Orders_CreatedAt Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
    UpdatedAt DateTimeOffSet(7),
    Constraint Fk_Orders_UserId Foreign Key(UserId) References Users(UserId),

    );

    Alter table Orders add constraint CHK_Orders_Status_New check (Status IN ('CREATED','PAYMENT_PENDING',
    'PAID','COD_PENDING','CONFIRMED','SHIPPED','DELIVERED','CANCELLED','PAYMENT_FAILED'));
 
    Alter table Orders Add PhoneNumber varchar(10);

    Alter table Orders Add FullName varchar(255);

    --insert into Orders Table--
    Insert into Orders (UserId, Status, TotalAmount, DeliveryAddress)values
    ('USR001', 'Pending', 24500.00, '12, MG Road, Bangalore, Karnataka'),
    ('USR002', 'Approval', 7800.00, '22, Anna Nagar, Chennai, Tamil Nadu'),
    ('USR003', 'Shipped', 17500.00, '45, Banjara Hills, Hyderabad, Telangana');

    ---get data ----
    select * from Orders;

    --fetch Data---


    /*OrderItems*/

    Create table OrderItems(
      OrderItemId INT NOT NULL IDENTITY(1,1) constraint Pk_OrderItems_OrderItemId Primary Key,
      OrderId INT NOT NULL,
      ProductId INT NOT NULL,
      Quantity INT NOT NULL DEFAULT 1,
      UnitPrice DECIMAL(14,2) NOT NULL,
      TotalPrice AS (Quantity * UnitPrice),

      Constraint Fk_OrderItems_OrderId Foreign Key(OrderId) References Orders(OrderId),
      Constraint Fk_OrderItems_ProductId  Foreign Key(ProductId) References Products(ProductId)
    );

    --insert into OrderItems--
    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
    VALUES
    (102, 1000, 2, 14500.00);

    --get data---
    select * from OrderItems;

    /* Payments*/
    Create Table Payments(
    PaymentId int identity(1000,1) not null Constraint Pk_Payments_PaymentId Primary Key,
    OrderId int not null,
    PaymentMethod varchar(50) not null,
    PaymentStatus varchar(50) not null,
    TransactionId varchar(15) not null,
    Amount decimal(15,2) not null,
    PaymentDate DateTimeOffSet(7) not null Constraint Df_Payments_Date Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
    IsActive Bit not null Default 1,

    Constraint Fk_Payments_OrderId Foreign Key(OrderId) References Orders(OrderId),
    Constraint Chk_Transaction Check(Len(TransactionId)=15),
    Constraint Chk_Amount Check(Amount>0)
    );

 

    --insert into payments table---
    INSERT INTO Payments ( OrderId, PaymentMethod, PaymentStatus, TransactionId, Amount)
    VALUES
    ( 100, 'Card', 'Success', 'TXN000000000001', 22300.00),
    ( 101, 'Upi', 'Failure', 'TXN000000000002', 17500.00),
    ( 102, 'Cash', 'Success', 'TXN000000000003', 29000.00);

    --get data ---
    select * from Payments;

    Create unique index UX_Payment_OrderId ON Payments(OrderId);

    /*create Delivery*/

    Create Table Delivery(
    DeliveryId int identity(1,1) Constraint Pk_Id Primary Key,
    OrderId int  not null,
    AssignedToTechId int  null,
    Status varchar(30),
    AssignedAt DateTimeOffSet(7) not null Constraint Df_AssingedAt default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
    DeliveryAt DateTimeOffSet(7) null,
    AssignedBy varchar(30) Default 'Admin',
    IsActive bit not null Default 1,

    Constraint Fk_Delivery_OrderId Foreign Key(OrderId) References Orders(OrderId),
    Constraint Fk_Delivery_AssignedTech Foreign Key(AssignedToTechId) References Technicians(TechnicianId),
    Constraint Chk_Delivery_Status Check(Status in('Delivered','Pending','OutForDelivery','Assigned'))

    );


    Alter table Delivery Add Constraint Chk_Delivery_Status Check(Status in('Delivered','Pending','OutForDelivery','Assigned'))


    ---create trigger on delivery---
    /*Create trigger tr_deliveryDate on Delivery
    after insert 
    as
    begin
     Update d set DeliveryAt=DateAdd(Day,7,d.AssignedAt) from Delivery d
     Inner join   inserted i on d.DeliveryId=i.DeliveryId
     where i.DeliveryAt is null
    end;*/



    --insert into data---
    INSERT INTO Delivery ( OrderId, AssignedToTechId, Status)
    VALUES
    ( 101, 100, 'Delivered');

    
    --get data--
    select * from Delivery;
    --fetch Data---

    /*Installations*/
    Create Table Installations(
        InstallationId varchar(18) not null Constraint Pk_Installations_Id Primary Key,
        OrderId int not null,
        CreatedAt datetimeoffset(7) not null Constraint Df_Installations_CreatedAt Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
        Status varchar(30) not null,
        CompletedDate datetimeoffset(7),
        Installation_Price decimal(14,2) null, 
        IsActive bit not null default 1,

        Constraint Fk_Installations_OrderId Foreign Key(OrderId) References Orders(OrderId),
        Constraint Chk_Installations_Status Check(Status in('Assigned','InProgress','Completed'))

    );


    
     --insert data---
     INSERT INTO Installations (InstallationId, OrderId, Status, ScheduledDate)values
    ('INST002', 100, 'Assigned', DATEADD(DAY, 5, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')));

    --Alter Table----
    Alter table Installations Add DeliveryId int  null Constraint Fk_Installations_DeliveryId Foreign Key(DeliveryId)
    References Delivery(DeliveryId) ;



    

    --get data---
    select * from Installations;

    /*installation technicians*/

     Create table InstallationTechnicians (
        Installation_Tech_Id int identity(1,1) Primary Key,
        InstallationId varchar(18) NOT NULL,
        TechnicianId int NOT NULL,
        Service_Price_At_Time decimal(12,2) NOT NULL,
        IsActive bit not null Default 1,
    );

   
 ALTER TABLE InstallationTechnicians ALTER COLUMN InstallationId VARCHAR(18) NOT NULL;

    --insert into data---
    INSERT INTO InstallationTechnicians (InstallationId, TechnicianId, Service_Price_At_Time)
    VALUES  
    ('INST002', 100, 250.00);

    select * from InstallationTechnicians;

    /*InstallationReports*/

      Create  table InstallationReports (
        ReportId int identity(1000,1) not null Constraint Pk_Reports PRIMARY KEY,
        InstallationId varchar(18) not null,
        Notes nvarchar(max) null,
        Image_Url nvarchar(1000) null,
        CheckList nvarchar(max) NULL,
        CreatedAt datetimeoffset(7) not null Constraint Df_Reports_CreatedAT Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
        IsActive bit not null default 1,


    );
     ALTER TABLE InstallationReports ALTER COLUMN InstallationId VARCHAR(18) NOT NULL;
    ALTER TABLE InstallationReports ADD CONSTRAINT FK_InstallationReports_Installation FOREIGN KEY (InstallationId) REFERENCES Installations(InstallationId);

    --insert into reports--
    INSERT INTO InstallationReports (InstallationId, Notes, Image_Url, CheckList, IsActive)
    VALUES
    ('INST003',
     'Initial inspection completed. All equipment delivered and ready for installation.',
     'https://example.com/images/ins00001_1.jpg',
     '{"PanelMounted": true, "InverterInstalled": false, "WiringChecked": false}',
     1);

    --get data--
    select * from  InstallationReports;
  
    
    /*Maintenance Requests*/
    CREATE TABLE MaintenanceRequests (
        RequestId varchar(8) not null Constraint Pk_Request Primary Key,
        UserId varchar(6) not null,
        Description nvarchar(max),
        Status nvarchar(50) not null Default 'Pending',
        EstimatedPrice DECIMAL(14,2) null, 
        CreatedAt datetimeoffset(7) not null Constraint Df_Request_CreatedAt Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
        IsActive bit not null default 1,
        CONSTRAINT FK_MaintRequests_User Foreign Key (UserId) REFERENCES Users(UserId),
        Constraint Chk_Request_Status Check(Status in('Pending','Assigned','Inprogress','Completed')),
        Constraint Chk_Request_Price Check(EstimatedPrice is null or EstimatedPrice>0)
    );

    

    --Alter table fo adding InstallationId---

    Alter table MaintenanceRequests Add InstallationId varchar(18) Constraint Fk_Request_InstallationsId Foreign Key
    References Installations(InstallationId) ;

   
    
    

    --insert into request---
    INSERT INTO MaintenanceRequests 
    (RequestId, UserId, Description, Status, EstimatedPrice, IsActive)
    VALUES
    ('REQ00001', 'USR001', 'Panels not working', 'Pending', NULL, 1),

    ('REQ00002', 'USR002', 'Efficinecy reduced', 'Assigned', 1500.00, 1),

    ('REQ00003', 'USR003', 'Wiring faults', 'Inprogress', 2500.00, 1);

    update MaintenanceRequests Set DeliveryId='DLV002' where UserId='USR002';
    update MaintenanceRequests Set InstallationId='INST002' where UserId='USR002';
    --get data--
    select * from MaintenanceRequests;

    /*Maintenance technicians*/

     Create table MaintenanceTechnicians (
        MaintenanceId int identity(1,1) Primary Key,
        RequestId varchar(8) not null,
        TechnicianId int NOT NULL,
        Service_Price_At_Time decimal(12,2) NOT NULL,
        IsActive bit not null Default 1,
        Constraint FK_MainTech_Request Foreign Key ( RequestId) References MaintenanceRequests(  RequestId),
        Constraint FK_MainTech_Technician Foreign Key (TechnicianId) References Technicians(TechnicianId)
    );

    Alter table MaintenanceTechnicians Drop FK_MainTech_Technician;
    
    --insert into data--
    INSERT INTO MaintenanceTechnicians 
    (RequestId, TechnicianId, Service_Price_At_Time, IsActive)
    VALUES
    ('REQ00001', 100, 750.00, 1), 
    ('REQ00002', 100, 1200.00, 1);

    --get data---
    select * from MaintenanceTechnicians;

    /*MaintenanceReports*/

      Create  table MaintenanceReports (
        ReportId int identity(1000,1) not null Constraint Pk_Maintenance_Reports PRIMARY KEY,
        RequestId varchar(8) not null,
        Notes nvarchar(max) null,
        Image_Url nvarchar(1000) null,
        ReplacedParts nvarchar(500) not null ,
        CreatedAt datetimeoffset(7) not null Constraint Df_Maintenance_Reports_CreatedAT Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
        IsActive bit not null default 1,
        CONSTRAINT FK_MaintenaceReports_RequestId Foreign Key (RequestId) References MaintenanceRequests (RequestId)

    );

    Insert into MaintenanceReports(RequestId, Notes, Image_Url, ReplacedParts)values
    ('REQ00001','Replaced damaged wiring and cleaned inverter connectors.','https://example.com/images/maint1.jpg','Wiring Kit, Connector Pins'),

    ('REQ00002','Panel cleaning done. No major issues found.','https://example.com/images/maint2.jpg','Cleaning Solution');

    --get Data--
    Select * from MaintenanceReports;

    /*Create Notification*/
    Create table Notifications(
      NotificationId int  identity(1,1) Constraint Pk_NotificationId Primary Key,
      UserId varchar(6) not null,
      Message nvarchar(1000) not null,
      Type nvarchar(100),
      CreatedAt datetimeoffset(7) not null Constraint Df_Notifications_CreatedAt Default SwitchOffSet(SysDateTimeOffSet(),'+05:30'),
      IsRead bit not null default 0,
      IsActive bit not null default 1,

      Constraint Fk_Notifications_UserId Foreign Key(UserId) References Users(UserId),
      Constraint Chk_Notification_Type Check(Type in('Order','Maintenance','System','Performance') or Type is null)
    );

    Alter table Notifications Add  Title varchar(100);

    Alter table Notifications drop constraint Chk_Notification_Type;

    --insert into Notifications---
    Insert into Notifications ( UserId, Message, Type, IsRead, IsActive)values

    ( 'USR001', 'A technician has been assigned to your maintenance request MR1001.', 'Maintenance', 0, 1),

    ( 'USR002', 'System update completed successfully.', 'System', 1, 1),

    ( 'USR003', 'Your solar panel performance has improved this week.', 'Performance', 0, 1),

    ( 'CKU123', 'You have unread notifications.', NULL, 0, 1);



    select * from Notifications;

    /*solar performance*/
      Create table SolarPerformance(
        PerformanceId INT IDENTITY(1000,1) PRIMARY KEY,
        UserId VARCHAR(6) NOT NULL,
        Voltage DECIMAL(10,3),
        CurrentFlow DECIMAL(10,3) CHECK (CurrentFlow >= 0 OR CurrentFlow IS NULL),
        Power DECIMAL(12,3),
        Efficiency DECIMAL(6,2),
        Generated_at DATETIMEOFFSET(7) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,

        CONSTRAINT Fk_SolarPerformance_UserId FOREIGN KEY(UserId) REFERENCES Users(UserId),
        CONSTRAINT Chk_Solar_Voltage CHECK (Voltage >= 0 OR Voltage IS NULL),
        CONSTRAINT Chk_Solar_Power CHECK (Power >= 0 OR Power IS NULL),
        CONSTRAINT Chk_Solar_Efficiency CHECK (Efficiency BETWEEN 0 AND 100 OR Efficiency IS NULL)
    );
    --Insert into table---
    INSERT INTO SolarPerformance (UserId, Voltage, CurrentFlow, Power, Efficiency, Generated_at)
    VALUES
    ('USR001', 38.500, 8.200, 315.400, 92.50, SYSDATETIMEOFFSET()),

    ('USR002', 40.100, 7.950, 318.795, 89.30, SYSDATETIMEOFFSET()),

    ('USR003', 37.800, 0.000, 0.000, NULL, SYSDATETIMEOFFSET()); 

    --get data---
    select * from SolarPerformance;

    Alter table Installations Add IsPanelCreated bit not null default 0;

    Alter table SolarPerformance Add PanelId int;

    Alter Table SolarPerformance Add Constraint Fk_SolarPerformance_PanleId Foreign key(PanelId) references Panel(PanelId);

    Alter table SolarPerformance Alter column PanelId int not null;

    Alter table SolarPerformance Alter column Temperature int ;

    Update SolarPerformance set Temperature=85 where Temperature is null;
  
    /*Invoice*/
      Create table Invoices (
        InvoiceId int identity(1,1) primary key,
        OrderId int not null,
        CustomerName varchar(100) not null,
        CustomerEmail varchar(256) not null,

        InvoiceDate datetimeoffset(7) not null 
            DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'India Standard Time'),
        Amount decimal not null,
        Tax decimal,
        TotalAmount decimal(14,2) not null,
        PaymentStatus varchar(50) not null,
        OrderStatus varchar(50) not null,
        PaymentMethod varchar(50) not null,

        IsActive BIT NOT NULL DEFAULT 1,

        CONSTRAINT FK_Invoices_Order  FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
    );

    Alter table Invoices Add PhoneNumber varchar(10);
    
    Create table OtpManager(
    OtpId int identity(1,1) constraint Pk_Otp_Id Primary Key,
    EmailId varchar(250) not null, 
    OtpText varchar(10) not null,
    OtpType varchar(20),
    Expiration DatetimeOffSet(7) not null,
    CreatedAt datetimeoffset(7) not null constraint df_Otp_CreatedAt default SwitchOffSet(SysDateTimeOffSet(),'+05:30'));
    Alter table OtpManager Add ResetToken uniqueIdentifier null ;

    
    CREATE TABLE ProductReviews (
    ReviewId INT IDENTITY PRIMARY KEY,
    ProductId INT NOT NULL,
    UserId VARCHAR(6) NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(1000) NULL,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_ProductReviews_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_ProductReviews_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


Alter table ProductReviews Add UpdateAt DateTimeOffSet(7) null;

EXEC sp_rename 'ProductReviews.UpdateAt','UpdatedAt','COLUMN';

Select * from  Orders where OrderId=170;

Select * from Delivery;
  CREATE TABLE Categories ( 
  CategoryId INT PRIMARY KEY IDENTITY(1,1), 
  CategoryName NVARCHAR(100) NOT NULL, 
  ParentCategoryId INT NULL, 
  IsActive BIT NOT NULL DEFAULT 1,
  CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryId) REFERENCES Categories(CategoryId) 
  );
  select * from Categories;
  select * from Products;

  Alter table Categories Add DeletedReason varchar(500);

     Alter table Categories Add DeletedAt DateTimeOffSet(7);
     

  Alter table Categories add Slug nvarchar(500) not null;
  Alter table Categories Add Description varchar(500);

  select * from Categories where ParentCategoryId is null;

  select * from Categories;
  /*Panel Table*/

  Create table Panel(
  PanelId int not null constraint pk_primary primary key  identity(1,1),
  UserId varchar(6) not null,
  PanelName varchar(100) not null,
  Capacity nvarchar(50)
  Constraint fk_Panel_UserId foreign key(UserId) references Users(UserId)
  );


  Create table RefreshTokens(
  TokenId int Identity(1,1) Constraint pk_TokenId Primary Key,

  UserId varchar(6) not null Constraint Rf_UserId Foreign Key references Users(UserId),
  RefreshToken varchar(Max) not null,
  CreatedAt DateTimeOffSet(7) not null,
  ExpirayData DateTimeOffSet 
  );

  Exec sp_rename 'RefreshTokens.ExpirayData','ExpiryDate';

    Alter table Panel Add IsActive bit default 1;
  
  CREATE TABLE AuditLog (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    UserId NVARCHAR(6) NULL,          
    UserRole NVARCHAR(50) NULL,

    TableName NVARCHAR(128) NOT NULL,
    Action NVARCHAR(50) NOT NULL,

    OldValues NVARCHAR(MAX) NULL,       
    NewValues NVARCHAR(MAX) NULL,       

    ChangedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

select * from AuditLog;


--AccessoriesMap----
CREATE TABLE AccessoryCategoryMapping (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceCategoryId INT NOT NULL,
    AccessoryCategoryId INT NOT NULL,
    IsAutoMapped BIT DEFAULT 0, 
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (SourceCategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (AccessoryCategoryId) REFERENCES Categories(CategoryId)
)

--AccessTable---
Create Table AccessPermissions(
AccessId int not null identity(1,1) Constraint pk_PrimaryAccessid Primary Key,
Icon varchar(300) not null,
RoleId int not null,
AccessName varchar(300) not null,
RoleName varchar(100) not null,
CreatedAt DateTimeOffSet(7) not null Constraint df_Access_Date Default(SwitchOffSet(SysDateTimeOffSet(),'+05:30')));

 Alter table AccessPermissions Add Constraint Fk_Access_RoleId Foreign Key(RoleId) References Roles(RoleId);

 Select * from AccessPermissions;

 Alter table AccessPermissions Add Url varchar(200) not null;

 Alter table AccessPermissions Add IsActive bit not null Constraint df_Acess_IsActive Default 1;

--Trigger---
Create or Alter Trigger Tr_AutoMappingAccesories on Categories
After Insert
As Begin
       Set NoCount On;
       Declare @parentAccesoryCategoryId Int;

       Select @parentAccesoryCategoryId=CategoryId from Categories where Slug='accessories';

       Insert into AccessoryCategoryMapping(SourceCategoryId,AccessoryCategoryId,IsAutoMapped)
       Select c.CategoryId as SourceCategoryId,c.CategoryId as AccessoryCategoryId,1
       from Categories c cross join inserted i 
       Where  i.ParentCategoryId = @parentAccesoryCategoryId
        AND c.ParentCategoryId IS NULL  
        AND c.CategoryId != @parentAccesoryCategoryId;
End;   

---Procedure 
Create or Alter Procedure sp_MapAccessoryToAllSystems @AccessoryCategoryId Int
As Begin
   Set NoCount On
   Insert into AccessoryCategoryMapping(SourceCategoryId,AccessoryCategoryId,IsAutoMapped)
   Select c.CategoryId,@AccessoryCategoryId,1 from Categories c
   Where c.ParentCategoryId is Null and c.CategoryId!=27
   And not Exists(Select 1 from AccessoryCategoryMapping m
   WHERE m.SourceCategoryId = c.CategoryId
              AND m.AccessoryCategoryId = @AccessoryCategoryId)
End;

Create or ALter Procedure sp_RebuildAccessoryMappings
   As begin
     Set NoCount On;
      Declare @AccesoryParentCategoryId Int;
      Select @AccesoryParentCategoryId=CategoryId from Categories Where Slug='accessories';

        Insert into AccessoryCategoryMapping(SourceCategoryId,AccessoryCategoryId,IsAutoMapped)
        Select c.CategoryId,a.CategoryId,1 from Categories c Cross Join Categories a
        Where a.ParentCategoryId = @AccesoryParentCategoryId
        AND c.ParentCategoryId IS NULL
        AND c.CategoryId != @AccesoryParentCategoryId and Not Exists(
        Select 1 from AccessoryCategoryMapping m Where m.SourceCategoryId=c.CategoryId
        and m.AccessoryCategoryId=a.CategoryId)
   End;

   Exec sp_RebuildAccessoryMappings;


   Alter table Products Add Unit Varchar(20);

   --Update Products with Procedure--
   Create Or Alter Procedure sp_Unit
   As Begin
     Set NoCount On
     Update Products set Unit =
               Case
                   When CategoryId in(35,36,37,38,39) Then 'Unit'

                   When Capacity Like '%[0-9]%' And PatIndex('%[^0-9]%',Capacity)>0
                   Then
                      SUBSTRING(Capacity,PATINDEX('%[^0-9]%',Capacity),Len(Capacity))

                   Else Capacity
            End;
                    
   End;

   Exec sp_Unit;


   --Trigger--
  Create or Alter Trigger Tr_Sp_Unit on Products 
  After Insert,Update 
  As Begin
     Set NoCount On;
     Update p set p.Unit=
      Case
          When p.CategoryId in (35,36,37,38,39) Then 'Unit'
          When p.Capacity Like '%[0-9]%' and PATINDEX('%[^0-9]%',p.Capacity)>0
           Then
               SUBSTRING(p.Capacity,PATINDEX('%[^0-9]%',p.Capacity),Len(p.Capacity))
          Else p.Capacity
      End

     from Products p inner join inserted i on i.ProductId=p.ProductId
  End;

 
select * from Products;
Select * from AccessoryCategoryMapping;
  Alter table Categories Add CreatedAt DatetimeOffSet(7) Constraint Df_Categories_CreatedAt 
    default(SwitchOffSet(SysDateTimeOffSet(),'+05:30'));

    Alter table Categories Add UpdatedAt DatetimeOffSet(7) null;
    
    select * from Roles;
    select * from Users;
    select * from Delivery;
    select * from Cart;
    select * from CartItems;
    select * from [dbo].[InstallationReports];
    select * from [dbo].[Installations];
    select * from [dbo].[InstallationTechnicians];
    select * from [dbo].[Invoices];
    select * from [dbo].[MaintenanceReports];
    select * from [dbo].[MaintenanceRequests];
    select * from [dbo].[MaintenanceTechnicians];
    select * from [dbo].[OrderItems];
    select * from [dbo].[Orders];
    select * from [dbo].[Payments];
    select * from [dbo].[Categories]
    select * from [dbo].[ProductImage];
    select * from [dbo].[Products];
    select * from ProductReviews;
    select * from [dbo].[SolarPerformance];
    select * from [dbo].[Technicians];
    select * from OtpManager;
    select * from Orders;
    select * from Panel;
    select * from AuditLog;
    select * from AccessPermissions;
    select * from Notifications;
    
    Insert into AccessPermissions(Icon,AccessName,RoleName,RoleId,Url)values
    ('','','',2,'');
    Update MaintenanceRequests set IsActive = 0 Where RequestId in('REQ00001','REQ00002','REQ00003') 
    Select * from Technicians where Specialization!='Installer';
    Update AccessPermissions set IsActive=0 where AccessId=4;
    Update AccessPermissions set RoleName='Customer' where RoleName='Custmer';
   select * from Users where EmailId like 'anand%';
   Select * from Cart where UserId='9Z9ZLF';
   Update CartItems set IsActive=0 where CartId=129;
   select * from CartItems where CartId=128;
      select * from CartItems where CartId=129;
      select * from Products where ProductId=1049;
   select * from Products where ProductName='Bificial Panels';
   Update Cart Set IsActive=0 where CartId in (132);

   Select * from Cart c inner join CartItems ci on c.CartId=ci.CartId Where c.CartId=131;

   Select * from Users where UserId in ('W2RGRR','Z6TWH4','HMUTJJ');


   select * from Categories;