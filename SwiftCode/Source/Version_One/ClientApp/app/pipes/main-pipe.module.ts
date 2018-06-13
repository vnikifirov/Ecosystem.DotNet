import { TitleCasePipe } from './title-case.pipe';
import { NgModule } from '@angular/core';
import {CommonModule} from "@angular/common";

@NgModule({
  declarations:[
    TitleCasePipe
  ],
  imports:[
    CommonModule
  ],
  exports:[
    TitleCasePipe
  ]
})

export class MainPipe{}
