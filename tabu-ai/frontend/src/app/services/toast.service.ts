import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
    id: number;
    message: string;
    type: 'success' | 'error' | 'warning' | 'info';
    duration: number;
}

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    private toasts: Toast[] = [];
    private toastsSubject = new BehaviorSubject<Toast[]>([]);
    public toasts$ = this.toastsSubject.asObservable();
    private nextId = 0;

    show(message: string, type: Toast['type'] = 'info', duration: number = 3000): void {
        const toast: Toast = {
            id: this.nextId++,
            message,
            type,
            duration
        };

        this.toasts.push(toast);
        this.toastsSubject.next([...this.toasts]);

        if (duration > 0) {
            setTimeout(() => this.remove(toast.id), duration);
        }
    }

    success(message: string, duration?: number): void {
        this.show(message, 'success', duration);
    }

    error(message: string, duration?: number): void {
        this.show(message, 'error', duration ?? 5000);
    }

    warning(message: string, duration?: number): void {
        this.show(message, 'warning', duration);
    }

    info(message: string, duration?: number): void {
        this.show(message, 'info', duration);
    }

    remove(id: number): void {
        this.toasts = this.toasts.filter(t => t.id !== id);
        this.toastsSubject.next([...this.toasts]);
    }
}
