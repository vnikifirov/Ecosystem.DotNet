export class Reg {
  public rgn: string;
  public center: string;
  public name: string;
  public namet: string;

  constructor({ rgn, center, name, namet }:
    {
      rgn?: string,
      center?: string,
      name?: string,
      namet?: string
    } = {}) {

    this.rgn = rgn || '';
    this.center = center || '';
    this.name = name || '';
    this.namet = namet || '';
  }
}
