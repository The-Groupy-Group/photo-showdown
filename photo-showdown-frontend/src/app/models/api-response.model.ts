export interface APIResponse <T>
{
  isSuccess:boolean;
  data:T;
  messsage:string;
}
