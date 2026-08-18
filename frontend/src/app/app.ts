import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AccessibilityBarComponent } from './shared/components/accessibility-bar/accessibility-bar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, AccessibilityBarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
