<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers;

Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::get('/items', [Controllers\ItemController::class, 'index']);
Route::get('/elderUser', [Controllers\ElderUserController::class, 'index']);
Route::get('/familyUser', [Controllers\FamilyUserController::class, 'index']);
Route::get('/familyLink', [Controllers\FamilyLinkContoroller::class, 'index']);
