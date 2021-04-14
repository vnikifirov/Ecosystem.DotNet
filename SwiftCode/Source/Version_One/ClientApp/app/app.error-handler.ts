import { ToastyService } from 'ng2-toasty';
import { ErrorHandler, Inject, NgZone } from "@angular/core";

// Loggers
import * as Raven from "raven-js";

export class AppErrorHandler implements ErrorHandler {
    // Tell Angular make DI a ToastyService to this class
    constructor(
        @Inject(ToastyService) private toastyService : ToastyService,
        private ngZone: NgZone,
    ) {}

    handleError(error: any): void {
        // Caputer errors
        Raven.captureException( error.originalError || error);

        // monkey-patching operation to  Ref: https://stackoverflow.com/questions/47123402/ngzone-or-zone-js-the-place-of-monkey-patching?rq=1
        this.ngZone.run( () => {
            this.toastyService.error({
                title: 'Error',
                msg: 'An unexpected error happen',
                theme: 'bootstrap',
                showClose: true,
                timeout: 5000
            });
        });
    }
}