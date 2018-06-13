import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppErrorHandler } from './app.error-handler';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { ToastyModule } from 'ng2-toasty';
import { NgxIntlTelInputModule } from 'ngx-intl-tel-input';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NKDatetimeModule } from 'ng2-datetime/ng2-datetime';

// Loggers
import * as Raven from "raven-js";

// Services
import { RecordsService } from './services/records.service';
import { RecordsUploadService } from './services/records-upload.service';

// Components
import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { RecordsComponent } from './components/records/records.component';
import { RecordsFormComponent } from './components/records-form/records-form.component';
import { RecordsUploadComponent } from './components/records-upload/records-upload.component';
import { TitleCasePipe } from './pipes/title-case.pipe';
import { MainPipe } from './pipes/main-pipe.module';

Raven
    .config('https://a4be56167fbb452792af63305147075a@sentry.io/305289')
    .install();

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        RecordsComponent,
        RecordsFormComponent,
        RecordsUploadComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        NgxIntlTelInputModule,
        BrowserModule,
        NKDatetimeModule,
        MainPipe,
        ToastyModule.forRoot(), // For Injecting to componets
        BsDropdownModule.forRoot(),
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'records', component: RecordsComponent },
            { path: 'upload-file', component: RecordsUploadComponent },
            { path: 'create', component: RecordsFormComponent },
            { path: 'update/:vkey', component: RecordsFormComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
     // Register our services
    providers: [
        { provide: ErrorHandler, useClass: AppErrorHandler },
        FormsModule,
        RecordsService,
        RecordsUploadService
    ]
})
export class AppModuleShared {
}
