<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers;

Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::get('/items', [Controllers\ItemController::class, 'index']);
Route::get('/elder-user', [Controllers\ElderUserController::class, 'index']);
Route::post('/elder-user', [Controllers\ElderUserController::class, 'store']);
Route::get('/family-user', [Controllers\FamilyUserController::class, 'index']);
Route::post('/family-user', [Controllers\FamilyUserController::class, 'store']);
Route::get('/family-link', [Controllers\FamilyLinkController::class, 'index']);
Route::post('/family-link', [Controllers\FamilyLinkController::class, 'store']);
Route::get('/games', [Controllers\GameController::class, 'index']);
Route::post('/games', [Controllers\GameController::class, 'store']);
Route::get('/play-log', [Controllers\PlayLogController::class, 'index']);
Route::post('/play-log', [Controllers\PlayLogController::class, 'store']);
Route::get('/videos', [Controllers\VideoController::class, 'index']);
Route::post('/videos', [Controllers\VideoController::class, 'store']);
Route::get('/user-item', [Controllers\UserItemsController::class, 'index']);
Route::post('/user-item', [Controllers\UserItemsController::class, 'store']);
Route::get('/animals', [Controllers\AnimalsController::class, 'index']);
Route::post('/animals', [Controllers\AnimalsController::class, 'store']);
Route::get('/user-pets', [Controllers\UserPetContoroller::class, 'index']);
Route::post('/user-pets', [Controllers\UserPetContoroller::class, 'store']);
Route::get('/watch-logs', [Controllers\WatchLogContoroller::class, 'index']);
Route::post('/watch-logs', [Controllers\WatchLogContoroller::class, 'store']);
