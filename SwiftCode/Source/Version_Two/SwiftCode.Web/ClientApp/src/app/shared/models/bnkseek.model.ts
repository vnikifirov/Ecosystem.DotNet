import { Pzn } from './pzn.model';
import { Uer } from './uer.model';
import { Reg } from './reg.model';
import { Tnp } from './tnp.model';

export class Bnkseek {
  public vkey: string;
  public pzn: Pzn;
  public uer: Uer;
  public reg: Reg;
  public ind: string;
  public tnp: Tnp;
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
      pzn?: Pzn,
      uer?: Uer,
      reg?: Reg,
      ind?: string,
      tnp?: Tnp,
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
    this.pzn = pzn || new Pzn();
    this.uer = uer || new Uer();
    this.reg = reg || new Reg();
    this.tnp = tnp || new Tnp();
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
