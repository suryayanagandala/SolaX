import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TypingService {

  startTypingEffect(
    fullText: string,
    callback: (val: string) => void,
    speed = 30
  ): Promise<string> {

    return new Promise((resolve) => {
      let index = 0;

      const interval = setInterval(() => {
        callback(fullText.substring(0, index + 1));
        index++;

        if (index === fullText.length) {
          clearInterval(interval);
          resolve(fullText);
        }
      }, speed);
    });
  }
}