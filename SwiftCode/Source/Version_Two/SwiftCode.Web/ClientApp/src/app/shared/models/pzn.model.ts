export class Pzn {
  public pzn: string;
  public imy: string;
  public name: string;

  constructor({ pzn, imy, name }:
    {
      pzn?: string,
      imy?: string,
      name?: string
    } = {}) {
    this.pzn = pzn || '';
    this.imy = imy || '';
    this.name = name || '';
  }
}
