import { Component } from '@angular/core';
import { ChatHub } from '../hubs/ChatHub';
import { BehaviorSubject } from 'rxjs';
import { Message } from 'src/hubs/ChatHub/models';

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
    _chatService.onMessageReceived$.subscribe((msg) => this.add(msg));
    this._chatService.tryConnect();
  }

  sendMessage(message: string) {
    if (message === 'clear') {
      this.messages$.next([]);
      return;
    }
    this._chatService.sendMessage(message).subscribe((msg) => this.add(msg));
  }

  private add(msg: Message | null) {
    if (msg?.user && msg?.content) {
      const m = this.messages$.value;
      m.push(msg);
      this.messages$.next([...m]);
    }
  }
}
