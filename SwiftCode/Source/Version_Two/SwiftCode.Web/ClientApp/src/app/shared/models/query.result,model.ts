import { Bnkseek } from './bnkseek.model';

// ? QueryResult holds response from the server
export class QueryResult {
  totalItems: number;
  items: Array<Bnkseek>;

  constructor({totalItems, items}:
  {
    totalItems: number,
    items: Array<Bnkseek>
  }) {
    this.totalItems = totalItems || 0;
    this.items = items || new Array<Bnkseek>();
  }
}
