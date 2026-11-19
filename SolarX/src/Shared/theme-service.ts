import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SharedThemeService {
  private isDarkModeSubject = new BehaviorSubject<boolean>(false);

isDarkMode$ = this.isDarkModeSubject.asObservable();

get isDarkModeValue(): boolean {
  return this.isDarkModeSubject.value;
}

setDarkMode(value: boolean) {
  this.isDarkModeSubject.next(value);
}

}
