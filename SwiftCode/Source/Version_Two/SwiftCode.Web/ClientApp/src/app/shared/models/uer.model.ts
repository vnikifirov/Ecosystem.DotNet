export class Uer {
  public uer: string;
  public uername: string;

  constructor({ uer, uername }:
  {
    uer?: string,
    uername?: string
  } = {}) {
    this.uer = uer || '';
    this.uername = uername || '';
  }
}
