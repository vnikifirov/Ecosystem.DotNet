import { Http } from '@angular/http';
import { Injectable, Inject } from '@angular/core';

import 'rxjs/add/operator/map';

@Injectable()
export class RecordsUploadService {
  private readonly recordsEndpoint;
  private readonly _baseUrl: string;

  constructor(
    private http: Http,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.recordsEndpoint = '/api/records';
    this._baseUrl = baseUrl || '';
  }

  upload(file: any) {
    const formData = new FormData();
    // ? key value should match arg name in controller
    formData.append('file', file);
    return this.http.post(this._baseUrl + this.recordsEndpoint + '/upload', formData);
  }
}
