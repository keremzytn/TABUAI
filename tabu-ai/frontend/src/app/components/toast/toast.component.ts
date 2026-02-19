import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Toast, ToastService } from '../../services/toast.service';

@Component({
    selector: 'app-toast',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="toast-container">
      <div *ngFor="let toast of toasts; trackBy: trackByFn"
           class="toast-item"
           [class.success]="toast.type === 'success'"
           [class.error]="toast.type === 'error'"
           [class.warning]="toast.type === 'warning'"
           [class.info]="toast.type === 'info'"
           (click)="dismiss(toast.id)">
        <div class="toast-icon">
          {{ getIcon(toast.type) }}
        </div>
        <div class="toast-message">{{ toast.message }}</div>
        <button class="toast-close" (click)="dismiss(toast.id); $event.stopPropagation()">✕</button>
      </div>
    </div>
  `,
    styles: [`
    .toast-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 10000;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 400px;
      width: calc(100vw - 40px);
    }

    .toast-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 14px 18px;
      border-radius: 14px;
      backdrop-filter: blur(20px);
      -webkit-backdrop-filter: blur(20px);
      border: 1px solid rgba(255, 255, 255, 0.15);
      cursor: pointer;
      animation: toastSlideIn 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) forwards;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
      transition: all 0.3s ease;
    }

    .toast-item:hover {
      transform: translateX(-4px);
    }

    .toast-item.success {
      background: rgba(16, 185, 129, 0.15);
      border-color: rgba(16, 185, 129, 0.4);
    }

    .toast-item.error {
      background: rgba(239, 68, 68, 0.15);
      border-color: rgba(239, 68, 68, 0.4);
    }

    .toast-item.warning {
      background: rgba(234, 179, 8, 0.15);
      border-color: rgba(234, 179, 8, 0.4);
    }

    .toast-item.info {
      background: rgba(139, 92, 246, 0.15);
      border-color: rgba(139, 92, 246, 0.4);
    }

    .toast-icon {
      font-size: 1.4rem;
      flex-shrink: 0;
    }

    .toast-message {
      flex: 1;
      color: #f8fafc;
      font-size: 0.95rem;
      font-weight: 500;
      line-height: 1.4;
    }

    .toast-close {
      background: none;
      border: none;
      color: rgba(255, 255, 255, 0.5);
      font-size: 0.85rem;
      cursor: pointer;
      padding: 4px;
      transition: color 0.2s;
      flex-shrink: 0;
    }

    .toast-close:hover {
      color: white;
    }

    @keyframes toastSlideIn {
      0% {
        opacity: 0;
        transform: translateX(100px) scale(0.8);
      }
      100% {
        opacity: 1;
        transform: translateX(0) scale(1);
      }
    }

    @media (max-width: 768px) {
      .toast-container {
        right: 10px;
        left: 10px;
        top: 10px;
        max-width: none;
        width: auto;
      }
    }
  `]
})
export class ToastComponent implements OnInit, OnDestroy {
    toasts: Toast[] = [];
    private sub!: Subscription;

    constructor(private toastService: ToastService) { }

    ngOnInit() {
        this.sub = this.toastService.toasts$.subscribe(t => this.toasts = t);
    }

    ngOnDestroy() {
        this.sub?.unsubscribe();
    }

    dismiss(id: number) {
        this.toastService.remove(id);
    }

    getIcon(type: Toast['type']): string {
        const icons = { success: '✅', error: '❌', warning: '⚠️', info: 'ℹ️' };
        return icons[type];
    }

    trackByFn(_: number, toast: Toast): number {
        return toast.id;
    }
}
