import { ChatHubConfiguration, ChatHubConfigurationParams } from './ChatHub.configuration';
import { ModuleWithProviders, NgModule } from '@angular/core';

@NgModule({
})
export class ChatHubModule {

  static forRoot(params: ChatHubConfigurationParams): ModuleWithProviders<ChatHubModule> {
    return {
      ngModule: ChatHubModule,
      providers: [
        {
          provide: ChatHubConfiguration,
          useValue: params
        }
      ]
    }
  }
}
