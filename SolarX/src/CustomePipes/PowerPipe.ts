import { Pipe,PipeTransform } from "@angular/core";

@Pipe({
    name:'powerTransform'
})
export class PowerFormatPipe implements PipeTransform{
    transform(value: any):string {
        if(value==null){
            return ''
        };

        if(value>=1_000_000){
            return(value/1_000_000).toFixed(2)+'MW'
        }

        if(value>=1_000){
            return(value/1_000).toFixed(2)+'kw'
        }

        if(value>=1_0000_00000){
            return (value/1_0000_00000).toFixed(2)+'GW'
        }
        return value.toFixed(2)+'W'
    }
}