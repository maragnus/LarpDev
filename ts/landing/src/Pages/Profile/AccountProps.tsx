import {Account} from "../../Protos/larp/accounts";

export interface AccountProps {
    account: Account
    updateAccount: (account: Account) => any;
}