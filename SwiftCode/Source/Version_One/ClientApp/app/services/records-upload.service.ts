import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import { RecordsUploadComponent } from './../components/records-upload/records-upload.component';

import "rxjs/add/operator/map";

@Injectable()
export class RecordsUploadService {

  constructor(private http: Http) { }

  upload(file: any) {
    var formData = new FormData();

    // key value should match arg name in controller
    formData.append('file', file);
    return this.http.post('/api/upload-file', formData);
  }
}
