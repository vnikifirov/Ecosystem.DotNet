import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Injectable()
export class RecordsService {
  _baseUrl: string = '';

  constructor(
    private _http: Http,
    @Inject('BASE_URL') baseUrl: string) {
      // this._baseUrl = baseUrl;
  }

  getAllRecords() {
      return this._http.get(this._baseUrl + '/api/records')
        .map( records => records.json());
  }

  getRecordByVKEY(vkey: string) {
    return this._http.get(this._baseUrl + '/api/records/' + vkey)
      .map( records => records.json());
  }

  create(newRecord) {
      return this._http.post(this._baseUrl + '/api/records', newRecord)
          .map((newRecord) => newRecord.json());
  }

  updateRecord(vkey: string, record) {
      return this._http.put(this._baseUrl + '/api/records/' + vkey, record)
          .map((updatedRecord) => updatedRecord.json());
  }

  deleteRecord(vkey : string) {
      return this._http.delete(this._baseUrl + "/api/records/" + vkey);
  }

  getPZNs() {
    return this._http.get(this._baseUrl + '/api/pzn')
      .map( pzns => pzns.json());
  }

  getREGs() {
    return this._http.get(this._baseUrl + '/api/reg')
      .map( regs => regs.json());
  }

  getTNPs() {
    return this._http.get(this._baseUrl + '/api/tnp')
      .map( tnps => tnps.json());
  }

  getUERs() {
    return this._http.get(this._baseUrl + '/api/uer')
      .map( tnps => tnps.json());
  }

  // errorHandler(error: Response) {
  //   console.log(error);
  //   return Observable.throw(error);
  // }
}
