export class Tnp {
  public tnp: string;
  public fullname: string;
  public shortname: string;

  constructor({ tnp, fullname, shortname }:
    {
      tnp?: string,
      fullname?: string,
      shortname?: string
  } = {}) {
    this.tnp = tnp || '';
    this.fullname = fullname || '';
    this.shortname = shortname || '';
  }
}
