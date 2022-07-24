import { Component } from '@angular/core';
import { ChatHub } from '../hubs/ChatHub';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'sigger-getting-started';

  // never use subjects as public properties
  readonly messages$ = new BehaviorSubject<Message[]>([]);

  constructor(private _chatService: ChatHub) {
    _chatService.onMessageReceived$.subscribe((x) => {
      if (x.user && x.message) {
        const m = this.messages$.value;
        m.push(x);
        this.messages$.next([...m]);
      }
    });
    this._chatService.tryConnect();
  }

  sendMessage(message: string) {
    if (message === 'clear') {
      this.messages$.next([]);
      return;
    }
    this._chatService.sendMessage(message).subscribe();
  }
}
