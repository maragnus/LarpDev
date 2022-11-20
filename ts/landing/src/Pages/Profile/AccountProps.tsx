import {Account} from "../../Protos/larp/accounts_pb";

export interface AccountProps {
    account: Account.AsObject
    updateAccount: (account: Account.AsObject) => any;
}