import { UploadFileComponent } from './../components/upload-file/upload-file.component';
import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import "rxjs/add/operator/map";

@Injectable()
export class FilesService {

  constructor(private http: Http) { }

  getFiles() {
    return this.http.get('/api/records')
      .map( files => files.json());
  }

  getPZNs() {
    return this.http.get('/api/records/pzns')
      .map( pzns => pzns.json());
  }

  upload(file: any) {
    var formData = new FormData();
    formData.append('file', file); // key value should match arg name in controller

    return this.http.post('/api/files/upload', formData)
      .map( records => records.json());
  }
}
