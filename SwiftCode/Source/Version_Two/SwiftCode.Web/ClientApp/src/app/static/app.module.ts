// Modules
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

// Components
import { BnkseekListComponent } from './../shared/components/bnkseek-list/bnkseek-list.component';
import { BnkseekFormComponent } from './../shared/components/bnkseek-form/bnkseek-form.component';
import { BnkseekUploadComponent } from './../shared/components/bnkseek-upload/bnkseek-upload.component';

// Main Components
import { AppComponent } from '../shared/components/app/app.component';
import { NavMenuComponent } from '../shared/components/nav-menu/nav-menu.component';
import { HomeComponent } from '../shared/components/home/home.component';

// Services
import { RecordsService } from '../core/services/records.service';
import { RecordsUploadService } from '../core/services/upload.service';

// Error Handlerы
import { AppErrorHandler } from '../core/app.error-handler';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    BnkseekListComponent,
    BnkseekFormComponent,
    BnkseekUploadComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: HomeComponent },
      { path: 'records/new', component: BnkseekFormComponent },
      { path: 'records', component: BnkseekListComponent },
      { path: 'records/upload', component: BnkseekUploadComponent },
      { path: 'records/edit/:vkey', component: BnkseekFormComponent },
    ])
  ],
  providers: [
    { provide: ErrorHandler, useClass: AppErrorHandler },
      FormsModule,
      RecordsService,
      RecordsUploadService
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
