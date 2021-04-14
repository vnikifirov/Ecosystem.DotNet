import { Router } from '@angular/router';
import { ToastyModule, ToastyService } from 'ng2-toasty';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RecordsService } from '../../services/records.service';
import { RecordsUploadService } from '../../services/records-upload.service';

@Component({
  selector: 'app-records-upload',
  templateUrl: './records-upload.component.html',
  styleUrls: ['./records-upload.component.css']
})
export class RecordsUploadComponent implements OnInit {
    @ViewChild('fileInput') fileInput; // ? In order to link to HTML element
    constructor(
      private uploadService : RecordsUploadService,
      private _toastyService : ToastyService,
      private _router : Router
    ) {}

    ngOnInit() {}

    uploadFile() : void {
      let nativeElement = this.fileInput.nativeElement ;

      if (nativeElement && nativeElement.files[0]) {
        this.uploadService.upload(nativeElement.files[0])
          .subscribe(
            success => {
              if (success.status == 200) {
                // ? Show a success message
                this._toastyService.success({
                  title: 'Успешно',
                  msg: 'Файл был загружен.',
                  theme: 'bootstrap',
                  showClose: true,
                  timeout: 5000
                });
                // ? Reset a file path
                nativeElement.value = '';
              }
            },
            err => {
              if (err.status == 400) {
                this._toastyService.error({
                  title: 'Ошибка',
                  msg: 'Неожиданная ошибка',
                  theme: 'bootstrap',
                  showClose: true,
                  timeout: 5000
                });
              }
            });
      }
    }
}
