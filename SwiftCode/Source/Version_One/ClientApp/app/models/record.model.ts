export interface Record {
  vkey: string;
  pzn: PZN;
  uer: UER;
  reg: REG;
  ind: string;
  tnp: TNP;
  nnp: string;
  adr: string;
  rkc: string;
  mamep: string;
  newnum: string;
  telef: string;
  regn: string;
  okpo: string;
  dtizm: Date;
  ksnp: string;
  datein: Date;
  datech: Date;
}

export interface SaveRecord {
  vkey: string;
  pzn: string;
  uer: string;
  rgn: string;
  ind: string;
  tnp: string;
  nnp: string;
  adr: string;
  rkc: string;
  namep: string;
  newnum: string;
  telef: string;
  regn: string;
  okpo: string;
  dtizm: Date;
  ksnp: string;
  datein: Date;
  datech: Date
}

export interface PZN {
  pzn: string;
  imy: string;
  name: string;
}

export interface REG {
  rgn: string;
  center: string;
  name: string;
  namet: string;
}

export interface TNP {
  tnp: string;
  fullname: string;
  shortname: string;
}

export interface UER {
  uer: string;
  uername: string;
}
