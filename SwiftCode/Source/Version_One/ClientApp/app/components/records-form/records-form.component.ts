// Routes
import { Router, ActivatedRoute } from '@angular/router';
// Core Components
import { Http } from '@angular/http';
import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { RecordsComponent } from '../records/records.component';

// Services
import { RecordsUploadService } from './../../services/records-upload.service';
import { RecordsService } from '../../services/records.service';

// Others
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/Observable/forkJoin'

// import Models
import {
  SaveRecord,
  PZN as PZN,
  UER as UER,
  REG as REG,
  TNP as TNP
} from "../../models/record.model";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastyService } from 'ng2-toasty';

@Component({
  selector: 'app-records-form',
  templateUrl: './records-form.component.html',
  styleUrls: ['./records-form.component.css']
})
export class RecordsFormComponent implements OnInit {
  // preferredCountries = ['ru', 'au', 'ru', 'gb'];
  recordForm: FormGroup;
  title:string = 'Создать';
  vkey: string;

  // Options for DateTime Picker
  datepickerOpts : {};

  // Models
  _pzns: PZN[];
  _uers: UER[];
  _regs: REG[];
  _tnps: TNP[];

  // _record:SaveRecord =<SaveRecord>{};
  _record: SaveRecord;

  constructor(
    private _toastyService : ToastyService,
    // private _fb: FormBuilder,
    private _recordsService : RecordsService,
    private _http: Http,
    private _avRoute: ActivatedRoute, // To read the record vkey from the URL
    private _router: Router
  ) {

    this._avRoute.params.subscribe( p => {
      this.vkey = p['vkey'];
    });

    //   if (this._avRoute.snapshot.params["VKEY"]) {
    //     this.VKEY = this._avRoute.snapshot.params["VKEY"];
    //   }
  }

  ngOnInit() {
    this._record = {
      vkey: '',
      pzn: '',
      uer: '',
      rgn: '',
      ind: '',
      tnp: '',
      nnp: '',
      adr: '',
      rkc: '',
      namep: '',
      newnum: '',
      telef: '',
      regn: '',
      okpo: '',
      dtizm : new Date(Date.now()),
      ksnp: '',
      datein: new Date(Date.now()),
      datech: new Date(Date.now())
    };

    this.datepickerOpts = {
      weekStart: 1,
      autoclose: true,
      todayBtn: 'linked',
      todayHighlight: true,
      assumeNearbyYear: true,
      format: 'dd/MM/yyyy'
    }

    let sourses = [
      this._recordsService.getPZNs(),
      this._recordsService.getREGs(),
      this._recordsService.getTNPs(),
      this._recordsService.getUERs(),
    ];

    // On edit only
    if (this.vkey) {
      this.title = "Редактировать";
      const base64VKEY = btoa(this.vkey)
      sourses.push(this._recordsService.getRecordByVKEY(base64VKEY));
    }
      // Sending Parallel Requests
    Observable.forkJoin(sourses).subscribe( data => {
      // Get data from Parallel Requests
      this._pzns = data[0] as PZN[] || [];
      this._regs = data[1] as REG[] || [];
      this._tnps = data[2] as TNP[] || [];
      this._uers = data[3] as UER[] || [];

      // On edit only
      if (this.vkey !== undefined) {
        this.setRecord(data[4] as SaveRecord || []);
      }
    },
    error => {
      if (error.status == 400) {
        this._router.navigate(['/home']);
      }
    });

    //   this.recordForm = this._fb.group({
    //     id: 0,
    //     name: ['', [Validators.required]],
    //     gender: ['', [Validators.required]],
    //     department: ['', [Validators.required]],
    //     city: ['', [Validators.required]]
    // })
  }

  setRecord(r : any) : void {
    this._record.vkey = r.vkey || '';
    this._record.pzn = r.pzn || '';
    this._record.uer = r.uer || '';
    this._record.rgn = r.rgn || '';
    this._record.ind = r.ind || '';
    this._record.tnp = r.tnp || '';
    this._record.nnp = r.nnp || '';
    this._record.adr = r.adr || '';
    this._record.rkc = r.rkc || '';
    this._record.namep = r.namep || '';
    this._record.newnum = r.newnum || '';
    this._record.telef = r.telef || '';
    this._record.regn = r.regn || '';
    this._record.okpo = r.okpo || '';
    this._record.dtizm = r.dtizm || {};
    this._record.ksnp = r.ksnp || '';
    this._record.datein = r.datein || {};
    this._record.datech = r.datech || {};
  }

  // onChange() {

  // }

  onSubmit() {
    // if (!this.recordForm.valid) { return; }

    if (this.title == "Создать") {
      console.log('CREATE_RESULT', this._record);
      this._recordsService.create(this._record)
        .subscribe((success) => {
          this._router.navigate(['/records']);
        },
        error => {
          if (error.status == 400) {
            this._toastyService.error({
              title: 'Ошибка',
              msg: 'Не удалось создать запись',
              theme: 'bootstrap',
              showClose: true,
              timeout: 5000
            });
          }
        });
      }

    if (this.title == "Редактировать") {
      console.log('EDIT_RESULT', this._record);
      const base64VKEY = btoa(this.vkey);

      this._recordsService.updateRecord(base64VKEY, this._record)
        .subscribe((success) => {
          this._router.navigate(['/records']);
        },
        error => {
          if (error.status == 400) {
            this._toastyService.error({
              title: 'Ошибка',
              msg: 'Не удалось отредактировать запись',
              theme: 'bootstrap',
              showClose: true,
              timeout: 5000
            });
          }
        });
      }
    }

  cancel() {
    this._router.navigate(['/records']);
  }
}
