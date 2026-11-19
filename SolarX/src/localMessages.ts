import { ResetPassword } from "./Components/reset-password/reset-password";

export const localMessages = {
   login: {
      sucess: 'Login successfully!',
      error: 'Invalid credentials'
   },

   Register: {
      registered: 'successfully registered!',
      error: 'Failed to registered',
   },

   Product: {
      sucess: 'Successfully added products',
      error: 'Something went wrong',
      fail:'fail to load products',
      update: 'sucessfully updated details',
      delete: 'sucessfully removed',
      ProductImages: {
         sucess: 'Successfully added',
         error: 'Something went wrong',
         update: 'sucessfully updated details',
         delete: 'sucessfully removed',
      }
   },

   cart: 'successfully added into cart',
   order: {
      sucess: 'Succesfully confirm order',
      address:'Please fill address',
      cancelOrder:'Order was successfully cancelled',
      error:'something went wrong'
   },

   error: {
      credentials: 'Invalid Credentials',
      fail: 'Something went wrong',
      server: '404 Not found',
      data:'Failed to load data',
   },

   payment: 'payment successfully',

   warnings:{
      login:'Please login first'
   },

   resetPassword:{
      expired:'Session expired please try again with otp',
      success:'sucessfully updated',
      fail:'Failed to update',
      required:'new password and repassword must be same'
   },

   otp:{
      invalid:'Invalid otp or otp expired',
      sucsess:'otp verified sucessfully',
      required:'email is required',
      fail:'something went wrong'
   },

   technician:{
      sucess:'sucessfully added',
      
   },

   delivery:{
      delivered:'Order sucessfully delivered',
      status:'Delivery status changed'
   },
   installation:{
      success:'Installation completed'
   }
}