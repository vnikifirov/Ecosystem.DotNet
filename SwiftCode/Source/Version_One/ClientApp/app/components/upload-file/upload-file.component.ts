import { FilesService } from './../../services/files.service';
import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.css']
})
export class UploadFileComponent implements OnInit {
  @ViewChild('fileInput') fileInput; // In order to link
  records: any[] = [];
  AllRecords: any[] = [];
  pzns: any[] = [];
  filter: any = {};

  constructor(private filesService : FilesService) {}

  ngOnInit() {
    this.filesService.getPZNs()
      .subscribe(pzns =>
        {
          this.pzns = pzns;
          console.log("RESPONSE_PZNs", this.pzns);
        });

    this.filesService.getFiles()
      .subscribe(records =>
        {
          this.records = this.AllRecords = records;
          console.log("RESPONSE_RECORDS", this.records);
        });
  }

  uploadFile() : void {
    let nativeElement = this.fileInput.nativeElement;

    if (nativeElement && nativeElement.files[0]) {
      this.filesService.upload(nativeElement.files[0])
        .subscribe(records =>
          {
            this.records = records;
            console.log("RESPONSE_RECORDS", records);
          },
          err => {
            console.log("RESPONSE_RESPONSE_ERR: ", err);
          });
    }
  }

  resetFilter() {
    this.filter = {};
    this.onFilterChange();
  }

  onFilterChange() {
    var records = this.AllRecords;

    if (this.filter.PZN) {
      records = records.filter( r => r.PZN == this.filter.PZN);
    }

    // Second filter
    // if () {

    // }

    this.records = records;
  }
}
