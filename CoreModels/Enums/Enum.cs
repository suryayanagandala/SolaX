using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Enums
{
    public enum RoleIds
    {
        Admin=1,
        Technician=2,
        Customer=3
    }

    public enum Discipline
    {
        Electrical = 1,
        Mechanical = 2,
        Civil = 3,
        Other=4
    }

    public enum Specialization
    {
        Installer = 1,
        Maintenance = 2,
        Electrician = 3,
        Supervisor = 4
    }

    public enum Qualification
    {
        ITI = 1,
        Diploma = 2,
        BTech = 3,
        MTech = 4
    }

    public enum ProductType
    {
        Mono,
        Poly,
        PERC,
        TOPCon,
        HJT,
        TFSP,
        Bifacial
    }

    public enum OrderStatus
    {
        CREATED,
        CONFIRMED,
        SHIPPED,
        DELIVERED,
        CANCELLED,
        PAYMENT_PENDING

    }

    public enum TechnicianRoles
    {
        Electrician,
        Maintenance,
        Installer,

    }

    public enum DeliveryStatus
    {
        Pending,
        Assigned,
        OutForDelivery,
        Delivered,
        InProgress
    }
    

    public enum InstallationStatus { Assigned, InProgress, Completed }

    public enum MaintenanceStatus { Pending, Assigned, Inprogress, Completed }

    public enum PaymentStatus
    {
        Pending,Success,Failure,
    }

    public enum PaymentMethod 
    {
        COD,
        UPI,
        CARD
    }

}
