import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';

import 'rxjs/add/operator/map';

// import Models
import { Bnkseek } from './../../shared/models/bnkseek.model';
import { QueryObj } from '../../shared/models/query.obj.model';

@Injectable()
export class RecordsService {
  private readonly _recordsEndpoint;
  private readonly _baseUrl: string;

  constructor(
    private _http: Http,
    @Inject('BASE_URL') baseUrl: string) {

      this._baseUrl = baseUrl || '';
      this._recordsEndpoint = '/api/records';
  }

  private hasValueAny(value: any): boolean {
    return value !== null && value !== undefined;
  }

  toQueryString(obj: {}): string {
    const parts = new Array<string>();

    // tslint:disable-next-line:forin
    for (const property in obj) {
      if (obj.hasOwnProperty(property)) {
        const value = <string>obj[property];
        // ? Check if selected value isn't undefined or null
        if (this.hasValueAny(value)) {
          parts.push(encodeURIComponent(property) + '=' + encodeURIComponent(value));
        }
      }
    }

    return parts.join('&');
  }

  getRecords(query: QueryObj) {
    return this._http.get(this._baseUrl + this._recordsEndpoint + '?' + this.toQueryString(query))
        .map( queryResult => queryResult.json());
  }

  getRecordByVKEY(vkey: string) {
    return this._http.get(this._baseUrl + this._recordsEndpoint + '/' + vkey)
      .map( res => res.json());
  }

  create(created: Bnkseek) {
      return this._http.post(this._baseUrl + this._recordsEndpoint, new)
          .map(res => res.json());
  }

  updateRecord(vkey: string, updated) {
      return this._http.put(this._baseUrl + this._recordsEndpoint + '/' + vkey, updated)
          .map(res => res.json());
  }

  deleteRecord(vkey: string) {
      return this._http.delete(this._baseUrl + this._recordsEndpoint + '/' + vkey);
  }

  getPZNs() {
    return this._http.get(this._baseUrl +  this._recordsEndpoint + '/pzn')
      .map( pzns => pzns.json());
  }

  getREGs() {
    return this._http.get(this._baseUrl + this._recordsEndpoint + '/reg')
      .map( regs => regs.json());
  }

  getTNPs() {
    return this._http.get(this._baseUrl + this._recordsEndpoint + '/tnp')
      .map( tnps => tnps.json());
  }

  getUERs() {
    return this._http.get(this._baseUrl + this._recordsEndpoint + '/uer')
      .map( tnps => tnps.json());
  }
}
