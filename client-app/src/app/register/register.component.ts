import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';
import { first } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  public progress!: number;
  public message!: string;
  profile_image: any = "";
  showPassword: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private authenticationService: AuthenticationService
  ) {

  }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(12), Validators.pattern('^[A-Za-z0-9_]+$')]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?!.*(.)\1{2}).{6,}$/)]],
      firstname: ['', [Validators.required, Validators.maxLength(60)]],
      lastname: ['', [Validators.maxLength(60)]],
      profileImage: ['']
    });
  }

  get f() { return this.registerForm.controls; }

  onSubmit() {
    this.submitted = true;
    console.log(this.f['username'].errors);
    if (this.registerForm.invalid) {
      return;
    }

    this.f['profileImage'].setValue(this.profile_image);

    this.error = '';
    this.loading = true;
    this.authenticationService.register(this.registerForm.value)
      .pipe(first())
      .subscribe({
        next: () => {
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
          this.router.navigate([returnUrl]);
        },
        error: error => {
          this.error = error;
          this.loading = false;
        }
      });
  }

  public uploadFile = (files: any) => {
    if (files.length === 0) {
      return;
    }
    let fileToUpload = <File>files[0];

    this.profile_image = "";
    this.progress = 0;
    // Check if file size is within limit
    if (fileToUpload.size > 5 * 1024 * 1024) {
      this.message = 'File size should not exceed 5MB.';
      return;
    }

    // Check if file type is allowed
    let allowedTypes = ['.jpg', '.jpeg', '.png', '.bmp'];
    let fileType = fileToUpload.name.substring(fileToUpload.name.lastIndexOf('.')).toLowerCase();
    if (!allowedTypes.includes(fileType)) {
      this.message = 'Only .jpg, .jpeg, .png, and .bmp file types are allowed.';
      return;
    }

    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    formData.append('type', 'Profile');

    this.http.post(`${environment.apiUrl}/api/Upload`, formData, { reportProgress: true, observe: 'events' })
      .subscribe((event: any) => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total!);
        else if (event.type === HttpEventType.Response) {
          console.log('uploadFile', event.body);
          if (event.body.status == true) {
            this.profile_image = event.body.path;
            this.message = 'Upload success.';
          } else {
            this.message = event.body.message;
          }
        }
      }, (error) => {
        console.error('uploadFile', error);
        this.message = 'Upload failed.';
      });
  }

  toggleShowPassword() {
    this.showPassword = !this.showPassword;
  }

}
