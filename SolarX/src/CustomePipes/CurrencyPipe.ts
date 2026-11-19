import { Pipe,PipeTransform } from "@angular/core";
@Pipe({
    name:'currencyPipe'
})
export class CurrencyPipe implements PipeTransform{

        transform(value: any):string {
            if(value==null){
                return ''
            }

            if(value>=1_000_0000){
                return (value/1_000_0000).toFixed(2)+'Cr'
            }

            if(value>=1_00_000){
                return (value/1_00_000).toFixed(2)+'L'
            }

            if(value>=1_000){
                return (value/1_000).toFixed(2)+'K'
            }

            else{
                return value.toFixed(2);
            }
        
            
        }

}