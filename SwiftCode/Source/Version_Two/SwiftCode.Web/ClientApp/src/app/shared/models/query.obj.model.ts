// ? QueryObj for making request to the server
export class QueryObj {
  public pzn: string;
  public regn: string;
  public newnum: string;
  public pageNumber: number;
  public pageSize: number;

  constructor({ pzn, regn, pageNumber, pageSize }:
  {
    pzn?: string,
    regn?: string,
    pageNumber?: number,
    pageSize?: number
  } = {}) {
    this.pzn = pzn || '';
    this.regn = regn || '';
    this.pageNumber = pageNumber || 0;
    this.pageSize = pageSize || 10;
  }
}
