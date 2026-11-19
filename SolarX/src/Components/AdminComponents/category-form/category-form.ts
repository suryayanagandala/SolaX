import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CategorieService } from '../../../Shared/categories';
import { CategoryForm, CategoryDto } from '../../../Models/Models';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [MaterialModule, CommonModule, ReactiveFormsModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss',
})
export class CategoryFormComponent implements OnInit {

  categoryId: number | null = null;
  categoryform!: FormGroup;
  categories: CategoryDto[] = [];

  isEditMode = false;


  selectedFile: File | null = null;
  imagePreview: string | null = null;

  constructor(
    private service: CategorieService,
    private toastr: ToastrService,
    private builder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.categoryform = this.builder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      parentId: [null]
    });
  }

  ngOnInit(): void {
    this.loadCategories();

    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.categoryId = +params['id'];
        this.isEditMode = true;

        if (this.categoryId) {
          this.loadCategoryById(this.categoryId);
        }
      }
    });
  }


  loadCategories() {
    this.service.getAllCategories().subscribe(res => {
      if (res.status) {
        this.categories = res.data ?? [];
      }
    });
  }

  //  LOAD SINGLE CATEGORY
  loadCategoryById(id: number) {
    this.service.getCategoryById(id).subscribe(res => {
      if (res.status) {
        this.categoryform.patchValue({
          name: res.data?.name,
          description: res.data?.description,
          parentId: res.data?.parentId,
          
        });

        
        if (res.data?.imageUrl) {
          this.imagePreview = `https://localhost:44362${res.data.imageUrl}`;
        }
      } else {
        this.toastr.error('Something went wrong');
      }
    });
  }


  onFileChange(event: any) {
    const file = event.target.files[0];

    if (!file) return;

 
    if (this.categoryform.value.parentId) {
      this.toastr.warning('Image allowed only for parent category');
      return;
    }

    this.selectedFile = file;

 
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  
  submit() {

    if (this.categoryform.invalid) {
      this.toastr.warning('Please fill all required fields');
      return;
    }

    const formValues = this.categoryform.getRawValue();

    const dto: CategoryForm = {
      name: formValues.name,
      description: formValues.description,
      parentId: formValues.parentId,
      image: this.selectedFile || undefined
    };


    if (!this.isEditMode) {
      this.service.createCategory(dto).subscribe(res => {
        if (res.status) {
          this.toastr.success('Category created successfully');
          this.resetForm();
          this.router.navigate(['/admin/category-list']);
        } else {
          this.toastr.error(res.error || 'Error occurred');
        }
      });
    }

  
    else if (this.categoryId) {
      this.service.updateParentCategory(this.categoryId, dto).subscribe(res => {
        if (res.status) {
          this.toastr.success('Category updated successfully');
          this.router.navigate(['/admin/category-list']);
        } else {
          this.toastr.error(res.error || 'Update failed');
        }
      });
    }
  }

 
  resetForm() {
    this.categoryform.reset();
    this.selectedFile = null;
    this.imagePreview = null;
  }
}

