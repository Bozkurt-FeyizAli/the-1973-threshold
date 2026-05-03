import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BurdenInput } from '../burden-input/burden-input';
import { TransmissionDisplay } from '../transmission-display/transmission-display';
import { BurdenPrompts } from '../burden-prompts/burden-prompts';
import { ApiService, TransitionResponse } from '../../services/api.service';
import { AudioService } from '../../services/audio.service';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

interface Memory {
  imageBase64: string;
  sentiment: string;
  timestamp: string;
}

const MEMORIES_KEY = 'knock_memories';
const MAX_MEMORIES = 8;

@Component({
  selector: 'app-radio-interface',
  standalone: true,
  imports: [CommonModule, BurdenInput, TransmissionDisplay, BurdenPrompts],
  templateUrl: './radio-interface.html',
  styleUrl: './radio-interface.css'
})
export class RadioInterface implements OnInit {
  @ViewChild(BurdenInput) burdenInputRef!: BurdenInput;

  isLoading = false;
  textResult = '';
  errorMessage = '';
  imageBase64 = '';
  detectedSentiment = '';
  memories: Memory[] = [];

  constructor(
    private apiService: ApiService,
    private audioService: AudioService
  ) {}

  ngOnInit() {
    try {
      const saved = localStorage.getItem(MEMORIES_KEY);
      if (saved) this.memories = JSON.parse(saved);
    } catch {}
  }

  onTransmit(userBurden: string) {
    this.isLoading = true;
    this.textResult = '';
    this.errorMessage = '';
    this.imageBase64 = '';
    this.detectedSentiment = '';

    this.audioService.startCrackle();

    const payload = {
      userBurden,
      timestamp: new Date().toISOString()
    };

    this.apiService.transmitBurden(payload).pipe(
      catchError(error => {
        return of({
          status: 'error',
          data: { farewellText: '', audioBase64: '', imageBase64: '', detectedSentiment: '', generationTimeMs: 0 },
          message: 'Radio frequency lost. Please try again.'
        } as TransitionResponse);
      })
    ).subscribe(response => {
      this.isLoading = false;
      this.audioService.stopCrackle();
      
      if (response.status === 'success') {
        this.textResult = response.data.farewellText;
        this.imageBase64 = response.data.imageBase64;
        this.detectedSentiment = response.data.detectedSentiment;
        this.audioService.playAmbient(this.detectedSentiment);
        if (response.data.audioBase64) {
          this.audioService.playAudio(response.data.audioBase64);
        }
        // Hafıza Duvarına ekle
        if (response.data.imageBase64) {
          this._addMemory(response.data.imageBase64, response.data.detectedSentiment);
        }
      } else {
        this.errorMessage = response.message || 'An unknown error occurred.';
      }
    });
  }

  private _addMemory(imageBase64: string, sentiment: string) {
    const entry: Memory = {
      imageBase64,
      sentiment,
      timestamp: new Date().toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: '2-digit' })
    };
    this.memories = [entry, ...this.memories].slice(0, MAX_MEMORIES);
    try {
      localStorage.setItem(MEMORIES_KEY, JSON.stringify(this.memories));
    } catch {}
  }

  clearMemories() {
    this.memories = [];
    localStorage.removeItem(MEMORIES_KEY);
  }

  sentimentLabel(s: string): string {
    if (s === 'POSITIVE') return '✦';
    if (s === 'NEGATIVE') return '✧';
    return '·';
  }

  onPromptSelect(text: string) {
    if (this.burdenInputRef) {
      this.burdenInputRef.userText = text;
    }
  }
}
