import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  editForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  UserProfile: any = {};

  public progress: number = 0;
  public message!: string;
  profile_image: any = "";
  showPassword: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private userService: UserService
  ) {

  }

  ngOnInit() {
    this.loadUserInfoByUserId();
    this.editForm = this.formBuilder.group({
      newPassword: ['', [Validators.minLength(6), Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?!.*(.)\1{2}).{6,}$/)]],
      firstname: ['', [Validators.required, Validators.maxLength(60)]],
      lastname: ['', [Validators.maxLength(60)]],
      profileImage: ['']
    });
  }

  loadUserInfoByUserId() {
    this.userService.getUserByUserId().pipe(first())
      .subscribe(res => {
        console.log("User Info:", res);
        this.UserProfile = res;
        this.profile_image = this.UserProfile.profileImage;
        this.f['firstname'].setValue(this.UserProfile.firstName);
        this.f['lastname'].setValue(this.UserProfile.lastName);
      });
  }

  get f() { return this.editForm.controls; }

  onSubmit() {
    this.submitted = true;
    if (this.editForm.invalid) {
      return;
    }

    this.f['profileImage'].setValue(this.profile_image);

    this.error = '';
    this.loading = true;
    this.userService.update(this.editForm.value)
      .pipe(first())
      .subscribe({
        next: () => {
          Swal.fire({
            title: 'Success!',
            text: "Refresh Profile!",
            icon: 'success',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'OK'
          }).then((result) => {
            this.loading = false;
            this.submitted = false;
            this.progress = 0;
            this.loadUserInfoByUserId();
          })
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
