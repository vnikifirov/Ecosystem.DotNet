import { ToastyService } from 'ng2-toasty';
import { Component, OnInit } from '@angular/core';
import { RecordsService } from '../../services/records.service';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

// Pipes
import { Pipe, PipeTransform } from '@angular/core';
import { TitleCasePipe } from '../../pipes/title-case.pipe';

// import Models
import { Record, PZN, UER, REG, TNP } from "../../models/record.model";
import { Filter } from '../../models/filter.model';

// Others
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/Observable/forkJoin'

@Component({
  selector: 'app-records',
  templateUrl: './records.component.html',
  styleUrls: ['./records.component.css']
})
export class RecordsComponent implements OnInit {
  allRecords: Record[];
  records: Record[];

  filter: Filter;
  pzns: Array<PZN>;

  //? Datepicke Pre-settings
  datepickerOpts: object;

  constructor(
    private _recordsService : RecordsService,
    public _http: Http,
    private _router: Router,
    private _toastyService: ToastyService
  ) {

    this.filter = {
      regname:'',
      pzn:'',
      newnum:''
    };

  }

  ngOnInit() {
    let sourses = [
      this._recordsService.getPZNs(),
      this._recordsService.getAllRecords()
    ];

      // Sending Parallel Requests
    Observable.forkJoin(sourses).subscribe( data => {
      // Get data from Parallel Requests
      this.pzns = data[0] as PZN[] || [];
      this.records = this.allRecords = data[1] as Record[] || [];

      this._toastyService.info({
        title: 'Информация',
        msg: 'Список с записями получен',
        theme: 'bootstrap',
        showClose: true,
        timeout: 5000
      });

    },
    error => {
      if (error.status == 400) {
        this._toastyService.error({
          title: 'Ошибка',
          msg: 'Не удалось загрузить записи',
          theme: 'bootstrap',
          showClose: true,
          timeout: 5000
        });
      }
    });

    // this.options = new DatePickerOptions();
    // this.datepickerOpts = {
    //   // TODO: Date from JSON
    //   // startDate: new Date("FROMJSON),
    //   autoclose: true,
    //   weekStart: 1,
    //   // todayBtn: 'linked',
    //   todayHighlight: true,
    //   assumeNearbyYear: true,
    //   forceParse: true,
    //   format: 'dd-MMM-yyyy',
    //   clearBtn: true,
    //   language: 'ru'
    // }
  }

  delete(vkey : string) {
      if (confirm("Вы уверены, что хотите удалить запись?")) {
          // ? Convert to base64
          let base64VKEY = btoa(vkey);
          this._recordsService.deleteRecord(base64VKEY)
            .subscribe((res) => {
              if (res.status == 200) {
                // ? Update a list of records
                this.allRecords = this.allRecords.filter( r => r.vkey.trim() !== vkey.trim());
                this.onFilterChange();
                // ? Show a success message
                this._toastyService.success({
                  title: 'Успешно',
                  msg: 'Запись была удалена.',
                  theme: 'bootstrap',
                  showClose: true,
                  timeout: 5000
                });
              }
            },
          error => {
            if (error.status == 400) {
              this._toastyService.error({
                title: 'Ошибка',
                msg: 'Не удалось удалить запись',
                theme: 'bootstrap',
                showClose: true,
                timeout: 5000
              });
            }
          });
      }
  }

  onFilterChange() {
    let records = this.allRecords;
    const filter:Filter = this.filter;

    // ? check if seacrh term undefined
    if (filter.newnum) {
      records = records.filter((r: Record) => r.newnum.trim().toLowerCase().includes(filter.newnum.trim().toLowerCase()));
    }

    if (filter.regname) {
      records = records.filter((r: Record) => r.reg.name.trim().toLowerCase().includes(filter.regname.trim().toLowerCase()));
    }

    if (filter.pzn) {
      records = records.filter( r => r.pzn.pzn.trim() === filter.pzn.trim());
    }

    this.records = records;
  }

  onResetFilter() {
    this.filter.newnum = this.filter.regname = this.filter.pzn = '';
    this.onFilterChange();
  }
}
