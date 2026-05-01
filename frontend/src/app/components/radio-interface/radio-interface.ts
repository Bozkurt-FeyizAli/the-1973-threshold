import { Component } from '@angular/core';
import { BurdenInput } from '../burden-input/burden-input';
import { TransmissionDisplay } from '../transmission-display/transmission-display';
import { ApiService, TransitionResponse } from '../../services/api.service';
import { AudioService } from '../../services/audio.service';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-radio-interface',
  imports: [BurdenInput, TransmissionDisplay],
  templateUrl: './radio-interface.html',
  styleUrl: './radio-interface.css'
})
export class RadioInterface {
  isLoading = false;
  textResult = '';
  errorMessage = '';
  imageBase64 = '';
  detectedSentiment = '';

  constructor(
    private apiService: ApiService,
    private audioService: AudioService
  ) {}

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
        if (response.data.audioBase64) {
          this.audioService.playAudio(response.data.audioBase64);
        }
      } else {
        this.errorMessage = response.message || 'An unknown error occurred.';
      }
    });
  }
}
