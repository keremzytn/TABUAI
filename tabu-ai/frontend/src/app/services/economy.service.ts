import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CoinBalance, PurchaseHintResult, PurchaseResult, ShopItem, InventoryResponse } from '../models/game.models';

@Injectable({
    providedIn: 'root'
})
export class EconomyService {
    private apiUrl = `${environment.apiUrl}/economy`;
    private balanceSubject = new BehaviorSubject<CoinBalance | null>(null);
    public balance$ = this.balanceSubject.asObservable();

    constructor(private http: HttpClient) {}

    getBalance(): Observable<CoinBalance> {
        return this.http.get<CoinBalance>(`${this.apiUrl}/balance`).pipe(
            tap(balance => this.balanceSubject.next(balance))
        );
    }

    purchaseHint(gameSessionId: string): Observable<PurchaseHintResult> {
        return this.http.post<PurchaseHintResult>(`${this.apiUrl}/purchase-hint`, { gameSessionId });
    }

    getShopItems(): Observable<ShopItem[]> {
        return this.http.get<ShopItem[]>(`${this.apiUrl}/shop`);
    }

    purchaseItem(itemId: string): Observable<PurchaseResult> {
        return this.http.post<PurchaseResult>(`${this.apiUrl}/purchase`, { itemId }).pipe(
            tap(result => {
                if (result.success && this.balanceSubject.value) {
                    this.balanceSubject.next({
                        ...this.balanceSubject.value,
                        balance: result.remainingCoins
                    });
                }
            })
        );
    }

    equipItem(type: string, itemId: string | null): Observable<any> {
        return this.http.post(`${this.apiUrl}/equip`, { type, itemId });
    }

    getInventory(): Observable<InventoryResponse> {
        return this.http.get<InventoryResponse>(`${this.apiUrl}/inventory`);
    }

    get currentBalance(): CoinBalance | null {
        return this.balanceSubject.value;
    }
}
