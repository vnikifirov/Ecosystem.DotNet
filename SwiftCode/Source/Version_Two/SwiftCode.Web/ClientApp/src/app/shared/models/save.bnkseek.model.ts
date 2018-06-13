export class SaveBnkseek {
  public vkey: string;
  public pzn: string;
  public uer: string;
  public reg: string;
  public ind: string;
  public tnp: string;
  public nnp: string;
  public adr: string;
  public rkc: string;
  public namep: string;
  public newnum: string;
  public telef: string;
  public regn: string;
  public okpo: string;
  public dtizm: Date;
  public ksnp: string;
  public datein: Date;
  public datech: Date;

  constructor({
    vkey, pzn, uer, reg,
    ind, tnp, nnp, adr,
    rkc, namep, newnum, telef,
    regn, okpo, dtizm, ksnp,
    datein, datech }:
    {
      vkey?: string,
      pzn?: string,
      uer?: string,
      reg?: string,
      ind?: string,
      tnp?: string,
      nnp?: string,
      adr?: string,
      rkc?: string,
      namep?: string,
      newnum?: string,
      telef?: string,
      regn?: string,
      okpo?: string,
      dtizm?: Date,
      ksnp?: string,
      datein?: Date,
      datech?: Date
  } = {}) {

    this.vkey = vkey || '';
    this.pzn = pzn || '';
    this.uer = uer || '';
    this.reg = reg || '';
    this.tnp = tnp || '';
    this.ind = ind || '';
    this.nnp = nnp || '';
    this.adr = adr || '';
    this.rkc = rkc || '';
    this.namep = namep || '';
    this.newnum = newnum || '';
    this.telef = telef || '';
    this.regn = regn || '';
    this.okpo = okpo || '';
    this.dtizm = dtizm || new Date();
    this.ksnp = ksnp || '';
    this.datein = datein || new Date();
    this.datech = datech || new Date();
  }
}
